using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GraphOptimizer : MonoBehaviour {
    public NeuralGraphVisualizer neuralGraphVisualizer;
    public Dish[] dishes;

    public struct Dish {
        public int id;
        public string dishName;
        public float [] penetrationOverTime;
        public float maxPenetration;


        public Dish (int id, string dishName, float[] penetrationOverTime) {
            this.dishName = dishName;
            this.id = id;
            this.penetrationOverTime = penetrationOverTime;
            maxPenetration = 0.1f;
            foreach (float penetration in penetrationOverTime) {
                maxPenetration = Mathf.Max(penetration, maxPenetration);
            }
        }
    }
    public int numberOfNetworks = 30;
    public float mutationAmount = 0.8f;
    
    public class DishGraph {
        public NeuralGraph neuralGraph;
        public InputNuron[] inputNurons;
        public NuronSource outputNuron;


        public float[] realPenetrationOverTime;
        public float[] projectedPenetrationOverTime;

        public float bestError = float.MaxValue;

        public void SetInputs(float[] penetrationOverTime, bool debug = false) {
            float maxValue = Mathf.Max(penetrationOverTime);

            if (projectedPenetrationOverTime == null) {
                projectedPenetrationOverTime = new float[penetrationOverTime.Length];
            }
            if (realPenetrationOverTime == null) {
                realPenetrationOverTime = new float[penetrationOverTime.Length];
            }

            for (int i = 0; i < projectedPenetrationOverTime.Length; i++) {
                projectedPenetrationOverTime[i] = penetrationOverTime[i];
                realPenetrationOverTime[i] = penetrationOverTime[i];
            }
            for (int yearOffset = 0; yearOffset < 4; yearOffset++) {
                for (int i = 0; i < inputNurons.Length; i++) {
                    if (debug)
                        Debug.Log("InputIndex: " + (i + yearOffset));
                    if (i == 0) {
                        inputNurons[i]._currentValue = 0f;// maxValue;// penetrationOverTime[0];
                    } else {
                        inputNurons[i]._currentValue = realPenetrationOverTime[i + yearOffset] - realPenetrationOverTime[i - 1 + yearOffset];
                    }
                    if (float.IsNaN(inputNurons[i]._currentValue) && debug)
                        Debug.Log("FOUND NAN: " + i + ", inputLength:" + inputNurons.Length + ", yo:" + yearOffset);
                    //inputNurons[i]._currentValue = penetrationOverTime[i];
                }

                float predictedOutput = realPenetrationOverTime[(inputNurons.Length - 1) + yearOffset] + outputNuron.currentValue;
                float expectedOutput = realPenetrationOverTime[inputNurons.Length + yearOffset];

                realPenetrationOverTime[inputNurons.Length + yearOffset] = predictedOutput;
                projectedPenetrationOverTime[inputNurons.Length + yearOffset] = predictedOutput;
            }
            for (int i = 0; i < projectedPenetrationOverTime.Length; i++) {
                realPenetrationOverTime[i] = penetrationOverTime[i];
            }
        }
    }

    public DishGraph[] dishGraphs;
    public DishGraph bestGraph;

    void Start() {
        TextAsset textAsset = Resources.Load<TextAsset>("processedPeneteration");
        string [] lines = textAsset.text.Split('\n');
        int num = 0;
        List<Dish> dishes = new List<Dish>();

        int previousID = -1;

        //for (int i = 0; i < lines.Length; i++) {
        //    string[] cells = lines[i].Split(',');

        //    if (cells.Length > 1) {
        //        int id;
        //        if (int.TryParse(cells[0], out id)) {
        //            if (id != previousID) {
        //                if (penetrationOverTime != null) {
        //                    int numberOfNonZeroes = 0;
        //                    for (int j = 0; j < penetrationOverTime.Count; j++) {
        //                        if (penetrationOverTime[j] > 0.0001f) {
        //                            numberOfNonZeroes++;
        //                        }
        //                    }
        //                    if (numberOfNonZeroes > 5f) {
        //                        bool addToDishes = true;
        //                        //foreach (float penetration in penetrationOverTime) {
        //                        //    if (penetration < 1) {
        //                        //        addToDishes = false;
        //                        //    }
        //                        //}
        //                        if (addToDishes)
        //                            dishes.Add(new Dish(previousID, cells[1], penetrationOverTime.ToArray()));
        //                    }
        //                }
        //                previousID = id;
        //                penetrationOverTime = new List<float>();
        //            }
        //            float penetrationNumber;
        //            float.TryParse(cells[3], out penetrationNumber);
        //            penetrationOverTime.Add(penetrationNumber);
        //        }
        //    }
        //    num += cells.Length;
        //}
        for (int i = 0; i < lines.Length; i++) {
            string[] cells = lines[i].Split(',');

            if (cells.Length == 11) {
                float[] penetrationOverTime = new float[11];

                for (int j = 0; j < penetrationOverTime.Length; j++) {
                    float.TryParse(cells[j], out penetrationOverTime[j]);
                }
                //if (dishes.Count < 50)
                    dishes.Add(new Dish(dishes.Count, dishes.Count.ToString(), penetrationOverTime));
                
            }
            num += cells.Length;
        }
        Debug.Log(dishes.Count);
        this.dishes = dishes.ToArray();



        const int numberOfInputs = 7;
        dishGraphs = new DishGraph[numberOfNetworks];
        
        for (int i = 0; i < dishGraphs.Length; i++) {
            dishGraphs[i] = new DishGraph();
            dishGraphs[i].neuralGraph = neuralGraphVisualizer.GetNeuralGraph();
            dishGraphs[i].neuralGraph.Mutate(Random.Range(0.5f, 20f));
            dishGraphs[i].inputNurons = new InputNuron[numberOfInputs];
            for (int j = 0; j < numberOfInputs; j++) {
                dishGraphs[i].inputNurons[j] = dishGraphs[i].neuralGraph.GetInputNuron(j.ToString());
            }
            dishGraphs[i].outputNuron = dishGraphs[i].neuralGraph.nuronSources[dishGraphs[i].neuralGraph.nodeKeyToIndex["Output"]];
        }
        dishGraphs[0].SetInputs(dishes[0].penetrationOverTime, true);
        bestGraph = dishGraphs[0];
    }
    bool saveResults;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            DoTestAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            saveResults = true;
        }
        if (checkParallelResults && parallelLoopResult.IsCompleted) {
            if (saveResults) {
                saveResults = false;
                SaveAsCSV();
            }
            checkParallelResults = false;
            if (bestGraph != null) {
                Debug.Log("Done!");
                Debug.Log("Best error: " + bestGraph.bestError / dishes.Length);
                for (int i = 1; i < dishGraphs.Length; i++) {
                    dishGraphs[i].bestError = float.MaxValue;
                    if (dishGraphs[i] != bestGraph && Mathf.Abs(mutationAmount) > 0.1f) {
                        dishGraphs[i].neuralGraph.CopyValues(bestGraph.neuralGraph);
                        dishGraphs[i].neuralGraph.Mutate(mutationAmount);
                    }
                }
            }
            DoTestAndUpdate();
        }
    }
    bool checkParallelResults;
    ParallelLoopResult parallelLoopResult;
    
    public void DoTestAndUpdate () {
        checkParallelResults = true;
        parallelLoopResult = Parallel.ForEach(dishGraphs, dishGraph => {
            float totalError = 0;
            for (int j = 0; j < dishes.Length; j++) {
                dishGraph.SetInputs(dishes[j].penetrationOverTime);
                for (int yearPrediction = 1; yearPrediction <= 4; yearPrediction++) {
                    int yearIndex = dishGraph.projectedPenetrationOverTime.Length - yearPrediction;

                    float error = Mathf.Abs(Mathf.Abs(dishGraph.projectedPenetrationOverTime[yearIndex] - dishGraph.realPenetrationOverTime[yearIndex]) / dishes[j].maxPenetration);


                    totalError += error;
                }
            }
            dishGraph.bestError = totalError;

            if (bestGraph == null || bestGraph.bestError > totalError) {
                lock (bestGraph) {
                    bestGraph = dishGraph;
                }
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
    public void SaveAsCSV() {
        string csvToSave = "";
        float[] inputArray = new float[11];
        for (int i = 0; i < dishes.Length; i++) {
            for (int k = 0; k < dishes[i].penetrationOverTime.Length; k++) {
                inputArray[k] = dishes[i].penetrationOverTime[k];
            }
            bestGraph.SetInputs(inputArray, i == 5);

            for (int yearPrediction = 0; yearPrediction < 4; yearPrediction++) {
                csvToSave += bestGraph.projectedPenetrationOverTime[bestGraph.projectedPenetrationOverTime.Length - (4 - yearPrediction)];
                if (yearPrediction != 3) {
                    csvToSave += ",";
                }
            }
            csvToSave += "\n";
        }
        Debug.Log("Saved csv!");
        Debug.Log(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));
        System.IO.File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/OutputCSV.csv", csvToSave);
    }
}
