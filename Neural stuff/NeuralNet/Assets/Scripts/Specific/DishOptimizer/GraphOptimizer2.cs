using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GraphOptimizer2 : MonoBehaviour {
    public NeuralGraphVisualizer neuralGraphVisualizer;
    public Dish[] dishes;

    public struct Dish {
        public float [] penetrationOverTime;
        public int[] expectedOutcomes;

        public Dish (int [] expectedOutcomes, float[] penetrationOverTime) {
            this.expectedOutcomes = expectedOutcomes;
            this.penetrationOverTime = penetrationOverTime;
        }
    }
    public int numberOfNetworks = 30;
    public float mutationAmount = 0.8f;
    
    public class DishGraph {
        public NeuralGraph neuralGraph;
        public InputNuron[] inputNurons;
        public NuronSource outputNuron;

        public float expectedOutput;
        public float predictedOutput;

        public float[] realPenetrationOverTime;
        public float[] projectedPenetrationOverTime;

        public float bestError = float.MaxValue;

        public void SetInputs (Dish dish, int epoch) {
            for (int i = 0; i < inputNurons.Length; i++) {
                if (i == 0) {
                    inputNurons[i]._currentValue = 0f;// penetrationOverTime[0];
                } else {
                    inputNurons[i]._currentValue = dish.penetrationOverTime[i + epoch];
                }
                //inputNurons[i]._currentValue = penetrationOverTime[i];
            }
            predictedOutput = dish.penetrationOverTime[inputNurons.Length - 1] + outputNuron.currentValue;
            expectedOutput = dish.expectedOutcomes[epoch];

            projectedPenetrationOverTime    = new float[inputNurons.Length + 1];
            realPenetrationOverTime         = new float[inputNurons.Length + 1];

            for (int i = 0; i < inputNurons.Length; i++) {
                projectedPenetrationOverTime[i] = dish.penetrationOverTime[i];
                realPenetrationOverTime[i]      = dish.penetrationOverTime[i];
            }
            projectedPenetrationOverTime[projectedPenetrationOverTime.Length - 1] = predictedOutput;
            realPenetrationOverTime     [projectedPenetrationOverTime.Length - 1] = expectedOutput;
        }
    }

    public DishGraph[] dishGraphs;
    public DishGraph bestGraph;

    void Start() {
        TextAsset textAsset = Resources.Load<TextAsset>("ExcelVModel");
        string [] lines = textAsset.text.Split('\n');
        int num = 0;
        List<Dish> dishes = new List<Dish>();

        int previousID = -1;

        List<float> penetrationOverTime = null;
        for (int i = 0; i < lines.Length; i++) {
            string[] cells = lines[i].Split(',');

            if (cells.Length == 14) {
                float[] penetrationDeltaOverTime = new float[10];
                int[] expectedOutcomes = new int[4];

                for (int j = 0; j < penetrationDeltaOverTime.Length; j++) {
                    float.TryParse(cells[j], out penetrationDeltaOverTime[j]);
                }
                for (int j = 0; j < expectedOutcomes.Length; j++) {
                    int.TryParse(cells[penetrationDeltaOverTime.Length + j], out expectedOutcomes[j]);
                }

                dishes.Add(new Dish(expectedOutcomes, penetrationDeltaOverTime));
            }
            num += cells.Length;
        }
        this.dishes = dishes.ToArray();


        
        const int numberOfInputs = 6;
        dishGraphs = new DishGraph[numberOfNetworks];
        
        for (int i = 0; i < dishGraphs.Length; i++) {
            dishGraphs[i] = new DishGraph();
            dishGraphs[i].neuralGraph = neuralGraphVisualizer.GetNeuralGraph();
            dishGraphs[i].neuralGraph.Mutate(Random.Range(0.1f, 10f));
            dishGraphs[i].inputNurons = new InputNuron[numberOfInputs];
            for (int j = 0; j < numberOfInputs; j++) {
                dishGraphs[i].inputNurons[j] = dishGraphs[i].neuralGraph.GetInputNuron(j.ToString());
            }
            dishGraphs[i].outputNuron = dishGraphs[i].neuralGraph.nuronSources[dishGraphs[i].neuralGraph.nodeKeyToIndex["Output"]];
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            DoTestAndUpdate();
        }
        if (checkParallelResults && parallelLoopResult.IsCompleted) {
            checkParallelResults = false;
            if (bestGraph != null) {
                Debug.Log("Done!");
                Debug.Log("Best error: " + bestGraph.bestError / dishes.Length);
                for (int i = 0; i < dishGraphs.Length; i++) {
                    if (dishGraphs[i] != bestGraph) {
                        dishGraphs[i].neuralGraph.CopyValues(bestGraph.neuralGraph);
                        dishGraphs[i].neuralGraph.Mutate(mutationAmount);
                    }
                }
            }
            Debug.Log(bestGraph.bestError);
            DoTestAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            SaveAsCSV();
        }
    }
    public void SaveAsCSV() {
        string csvToSave = "";
        for (int i = 0; i < dishes.Length; i++) {
            for (int j = 0; j < 4; j++) {
                bestGraph.SetInputs(dishes[i], j);

                csvToSave += bestGraph.predictedOutput;
                if (j != 3) {
                    csvToSave += ",";
                }
            }
            csvToSave += "\n";
        }
        Debug.Log(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));
        System.IO.File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/OutputCSV.csv", csvToSave);
    }

            bool checkParallelResults;
    ParallelLoopResult parallelLoopResult;
    
    public void DoTestAndUpdate () {
        bestGraph = dishGraphs[0];
        checkParallelResults = true;
        parallelLoopResult = Parallel.ForEach(dishGraphs, dishGraph => {
            float totalError = 0f;
            for (int j = 0; j < dishes.Length; j++) {
                for (int i = 0; i < 4; i++) {
                    dishGraph.SetInputs(dishes[j], i);
                    float output = dishGraph.outputNuron.currentValue;
                    float error = Mathf.Abs(dishGraph.predictedOutput - dishGraph.expectedOutput);

                    totalError += error;
                }
            }
            dishGraph.bestError = totalError;

            if (bestGraph.bestError > totalError) {
                lock (bestGraph) {
                    bestGraph = dishGraph;
                }
                bestGraph = dishGraph;
            }
        });

        //for (int i = 0; i < dishGraphs.Length; i++) {
        //    float totalError = 0f;

        //    for (int j = 0; j < dishes.Length; j++) {
        //        dishGraphs[i].SetInputs(dishes[j].penetrationOverTime);
        //        float output = dishGraphs[i].outputNuron.currentValue;
        //        float error = Mathf.Abs(dishGraphs[i].expectedOutput - output);
        //        totalError += error;
        //    }
        //    if (bestError > totalError) {
        //        bestError = totalError;
        //        bestGraph = dishGraphs[i];
        //    }
        //}
        
    }
}
