using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;

public class DishGraphVisualizer : MonoBehaviour {
    public int dishIndex = 0;

    public GraphChartBase graph;
    //public GraphOptimizer graphOptimizer;
    public GraphOptimizer graphOptimizer;

    float lastBest;
    float lastDishIndex;

    private void Update() {
        if (graphOptimizer.bestGraph != null) {
            if (graphOptimizer.bestGraph.bestError != lastBest || lastDishIndex != dishIndex) {
                lastDishIndex = dishIndex;
                lastBest = graphOptimizer.bestGraph.bestError;
                GraphOptimizer.Dish dish = graphOptimizer.dishes[dishIndex];
                graphOptimizer.bestGraph.SetInputs(dish.penetrationOverTime);
                UpdateGraph(graphOptimizer.bestGraph.realPenetrationOverTime, graphOptimizer.bestGraph.projectedPenetrationOverTime);
            }
        }
    }
    public void UpdateGraph (float [] realPoints, float [] expectedPoints) {
        if (graph != null) {
            graph.HorizontalValueToStringMap[0.0] = "Zero"; // example of how to set custom axis strings
            graph.DataSource.StartBatch();
            graph.DataSource.ClearCategory("Real");
            graph.DataSource.ClearCategory("Projected");

            for (int i = 0; i < realPoints.Length; i++) {
                graph.DataSource.AddPointToCategory("Real", i, realPoints[i]);
            }
            for (int i = 0; i < expectedPoints.Length; i++) {
                graph.DataSource.AddPointToCategory("Projected", i, expectedPoints[i]);
            }
            
            graph.DataSource.EndBatch();
        }
    }
}
