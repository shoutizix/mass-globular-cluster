﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FastNBodySlideController : SimulationSlideController
{
    private FastNBodySimulation sim;
    private NBodyPrefabs prefabs;

    [Header("Reset Properties")]
    [SerializeField] private bool virializeOnReset;

    [Header("Object Visibility")]
    [SerializeField] private bool centerOfMass;
    [SerializeField] private bool coordinateOrigin;
    [SerializeField] private bool angularMomentumVector;
    [SerializeField] private bool lights;

    [Header("Bloom")]
    [SerializeField] private GameObject globalVolume;
    [SerializeField] private bool bloom;

    [Header("Equations / Displays")]
    [SerializeField] private RectTransform panelRadialVelocity;
    [SerializeField] private MeterFill meterRadialVelocity;
    [SerializeField] private FadeOutUI handRotate;

    [Header("Buttons")]
    [SerializeField] private Button measureRadialVelocityButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button fitNormalButton;

    [Header("Sliders")]
    [SerializeField] private Slider numberSlider;
    [SerializeField] private Slider distanceSlider;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider speedSigmaSlider;

    [Header("Materials")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material glowMaterial;

    [Header("Vectors")]
    [SerializeField] private GameObject velocityPrefab;
    [SerializeField] private float lengthVectors = 5;

    [Header("Body Labels")]
    [SerializeField] private List<Sprite> bodyLabels;

    [Header("Graphs")]
    [SerializeField] private DynamicGraph graph;
    [SerializeField] private DynamicGraph graphX;
    [SerializeField] private bool normalDistributionOnGraphX = false;
    [SerializeField] private bool drawStdDeviationOnGraphX = false;
    [SerializeField] private DynamicGraph graphY;
    [SerializeField] private bool normalDistributionOnGraphY = false;
    [SerializeField] private bool drawStdDeviationOnGraphY = false;
    [SerializeField] private DynamicGraph graphZ;
    [SerializeField] private bool normalDistributionOnGraphZ = false;
    [SerializeField] private bool drawStdDeviationOnGraphZ = false;
    [SerializeField] private DynamicGraph graphTotal;
    [SerializeField] private bool normalDistributionOnGraphTotal = false;
    [SerializeField] private bool drawStdDeviationOnGraphTotal = false;
    [SerializeField] private Color colorNormalFit = Color.black;
    [SerializeField] private Color colorStandardDeviation = Color.black;
    [SerializeField] private float animationNormalDuration = 2f;
    [SerializeField] private float animationStandardDeviationDuration = 2f;
    [SerializeField] private float waitTimeIteration = 0.15f;
    [SerializeField] private float waitTimeBeforePlotNormalGraph = 1f;
    [SerializeField] private Color colorBorders = Color.black;

    [Header("Body Interactions")]
    [SerializeField] private bool bodiesInteractable = false;
    [SerializeField] private float reducedAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;

    [Header("Computed Sigma")]
    [SerializeField] private TextMeshProUGUI sigmaValue;
    [SerializeField] private TextMeshProUGUI sigmaValueX;
    [SerializeField] private TextMeshProUGUI sigmaValueY;
    [SerializeField] private TextMeshProUGUI sigmaValueZ;
    [SerializeField] private TextMeshProUGUI sigmaValueTotal;

    [Header("Computed Mass")]
    [SerializeField] private DisplayNextToText solarMassImage;
    [SerializeField] private DisplayNextToText powerText;
    [SerializeField] private TextMeshProUGUI massText;

    private HashSet<RectTransform> equations;
    private HashSet<Button> buttons;
    private HashSet<Slider> sliders;
    private Camera mainCamera;
    // Connector collection when computing U visually (and disabling)
    private HashSet<GameObject> connectors;
    private Vector velocityVector = default;
    private Arrow velocityArrow = default;

    // Mesure of the radial velocity
    private List<Vector2> listVelocityAndCount = new List<Vector2>();

    private Color colorZAxisBlue = new Color(0.3568628f, 0.6705883f, 0.945098f, 1);
    private Color colorYAxisGreen = new Color(0.5137255f, 0.8823529f, 0.7098039f, 1);


    private void Awake()
    {
        sim = (FastNBodySimulation)simulation;
        if (!simulation.TryGetComponent(out prefabs))
        {
            Debug.LogWarning("Did not find an NBodyPrefabs component");
        }

        // Get main camera reference
        mainCamera = Camera.main;

        // Collect all assigned buttons
        buttons = new HashSet<Button>();
        if (measureRadialVelocityButton)
        {
            buttons.Add(measureRadialVelocityButton);
        }
        if (resetButton)
        {
            buttons.Add(resetButton);
        }
        if (fitNormalButton)
        {
            buttons.Add(fitNormalButton);
        }

        // Collect equation images
        equations = new HashSet<RectTransform>();

        // Collect all assigned sliders
        sliders = new HashSet<Slider>();
        if (numberSlider)
        {
            sliders.Add(numberSlider);
        }
        if (distanceSlider)
        {
            sliders.Add(distanceSlider);
        }
        if (speedSlider)
        {
            sliders.Add(speedSlider);
        }
        if (speedSigmaSlider)
        {
            sliders.Add(speedSigmaSlider);
        }
    }

    public override void InitializeSlide()
    {
        if (!prefabs)
        {
            return;
        }

        sim.CustomReset(true, true, virializeOnReset);

        prefabs.SetCenterOfMassVisibility(centerOfMass);
        prefabs.SetCoordinateOriginVisibility(coordinateOrigin);
        prefabs.SetAngularMomentumVectorVisibility(angularMomentumVector);
        prefabs.SetLightsVisibility(lights);

        ResetBodyMaterials();
        HideBodyLabels();
        SetButtonsInteractivity(true, false);
        ShowAllUI();
        HideTextPanels();
        SetBloomVisibility(bloom);

        sim.SetInteractable(bodiesInteractable);
        CheckDistributionAllGraphs();
        if (speedSigmaSlider)
        {
            sim.SetSpeedDeviation(speedSigmaSlider.value);
        }
        UpdateMassText();

        if (startButton && !autoPlay)
        {
            foreach (Transform child in startButton.transform)
            {
                child.gameObject.SetActive(child.name == "Start Text");
            }
        }

        if (handRotate)
        {
            handRotate.TriggerReset();
        }

        if (solarMassImage)
        {
            solarMassImage.UpdateUnitPosition();
        }

        if (sigmaValueX)
        {
            UpdateSigmaText(sigmaValueX);
        }

        if (sigmaValueY)
        {
            UpdateSigmaText(sigmaValueY);
        }

        if (sigmaValueZ)
        {
            UpdateSigmaText(sigmaValueZ);
        }

        if (sigmaValueTotal)
        {
            // Compute the total sigma
            float totalSigma = sim.ComputeTotalSigma();
            UpdateSigmaText(sigmaValueTotal, totalSigma, true);
        }
    }

    public void Reset()
    {
        sim.CustomReset(true, true, virializeOnReset);
        if (autoPlay)
        {
            sim.Resume();
        }
        else
        {
            sim.Pause();
            SetSliderVisibility(true);
        }

        if (startButton)
        {
            foreach (Transform child in startButton.transform)
            {
                child.gameObject.SetActive(child.name == "Start Text");
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (connectors != null)
        {
            foreach (GameObject connector in connectors)
            {
                Destroy(connector);
            }
        }
        if (velocityVector)
        {
            Destroy(velocityVector.gameObject);
            velocityVector = null;
        }
    }

    private void CheckDistributionAllGraphs()
    {
        float mean = sim.GetMeanSpeed();
        float sigma = sim.lastComputedSigma;

        if (normalDistributionOnGraphX)
        {
            // If lastComputedSigma is the sigma value displayed
            // Thus if it equals 0 then it should plot the graph
            graphX.Clear();
            if (sigma != 0)
            {
                DrawNormalCurve(graphX, colorNormalFit, mean, sigma);
                if (drawStdDeviationOnGraphX)
                {
                    DrawStandardDeviation(graphX, Color.red, sigma);
                }
            }
        }

        if (normalDistributionOnGraphY)
        {
            graphY.Clear();
            if (sigma != 0)
            {
                DrawNormalCurve(graphY, colorNormalFit, mean, sigma);
                if (drawStdDeviationOnGraphY)
                {
                    DrawStandardDeviation(graphY, colorYAxisGreen, sigma);
                }
            }
        }

        if (normalDistributionOnGraphZ)
        {
            graphZ.Clear();
            if (sigma != 0)
            {
                DrawNormalCurve(graphZ,colorNormalFit, mean, sigma);
                if (drawStdDeviationOnGraphZ)
                {
                    DrawStandardDeviation(graphZ, colorZAxisBlue, sigma);
                }
            }
        }

        if (normalDistributionOnGraphTotal)
        {
            float sigmaTotal = sim.ComputeTotalSigma();

            // If sigma total equals 0 then don't draw the graph to avoid error
            // (should only happen when user doesn't fit normal distribution on slide measure velocity) 
            if (sigmaTotal != 0)
            {
                graphTotal.Clear();
                DrawNormalCurve(graphTotal,colorNormalFit, mean, sigmaTotal);
                if (drawStdDeviationOnGraphTotal)
                {
                    DrawStandardDeviation(graphTotal, Color.gray, sigmaTotal);
                }
            }
        }
    }

    // Called by Button
    public void FitNormalDistributionVisually()
    {
        SetButtonsInteractivity(false);
        simulation.Pause();
        StartCoroutine(AnimationNormalDistribution(animationNormalDuration));
    }

    // Called by Button
    public void ComputeRadialVelocityVisually()
    {
        sim.CustomReset(false, true, false);
        UpdateRadialVelocityMeter(0);
        SetButtonsInteractivity(false);
        simulation.Pause();
        sim.ComputeRadialVelocity();
        StartCoroutine(LoopOverBodiesMeasureRadialVelocity(sim.RadialVelocity));
    }

    public void ClearVelocitiesVectors()
    {
        if (prefabs.positionVectorVelocityX) prefabs.positionVectorVelocityX.SetComponents(Vector3.zero);
        if (prefabs.positionVectorVelocityY) prefabs.positionVectorVelocityY.SetComponents(Vector3.zero);
        if (prefabs.positionVectorVelocityZ) prefabs.positionVectorVelocityZ.SetComponents(Vector3.zero);
    }

    public void DisplayBodyVelocitiesAtIndex(int index)
    {
        Vector3 velocity = sim.GetVelocity(index);
        Vector3 pos = prefabs.bodies[index].position;

        // Simulation Part (Vectors)
        prefabs.positionVectorVelocityX.transform.position = pos;
        prefabs.positionVectorVelocityX.SetComponents(Vector3.right * velocity.x);

        prefabs.positionVectorVelocityY.transform.position = pos;
        prefabs.positionVectorVelocityY.SetComponents(Vector3.up * velocity.y);

        prefabs.positionVectorVelocityZ.transform.position = pos;
        prefabs.positionVectorVelocityZ.SetComponents(Vector3.forward * velocity.z); 

        ChangeAlphaOtherBodies(index, true);

        // UI Part (Graph Part)

        // When lastComputedSigma equals 0 the normal curve is not drawn
        // So we should remove the last line without the check
        if (sim.lastComputedSigma == 0)
        {
            graphX.ClearLastLine();
            graphY.ClearLastLine();
            graphZ.ClearLastLine();
        } else
        {
            // First check if the last line is not null to avoid crashes
            if (graphX.GetLastLine())
            {
                if (graphX.GetLastLine().name != "Line Standard deviation")
                {
                    CheckClearLastLine(graphX);
                }
            }
            if (graphY.GetLastLine())
            {
                if (graphY.GetLastLine().name != "Line Standard deviation")
                {
                    CheckClearLastLine(graphY);
                }
            }
            if (graphZ.GetLastLine())
            {
                if (graphZ.GetLastLine().name != "Line Standard deviation")
                {
                    CheckClearLastLine(graphZ);
                }
            }
        }

        // Display the velocity along each axis on the corresponding graph
        graphX.CreateLine(Color.red, true, "");
        graphX.PlotPointOnLastLine(Vector2.right * velocity.x);

        graphY.CreateLine(colorYAxisGreen, true, "");
        graphY.PlotPointOnLastLine(Vector2.right * velocity.y);

        graphZ.CreateLine(colorZAxisBlue, true, "");
        graphZ.PlotPointOnLastLine(Vector2.right * velocity.z);
    }

    // If lower is true then it reduces the alpha to reducedAlpha
    // If lower is false then it increase the alpha to maxAlpha
    public void ChangeAlphaOtherBodies(int index, bool lower)
    {
        for (int i = 0; i < prefabs.bodies.Count; i++)
        {
            if (i != index)
            {
                Renderer rend = prefabs.bodies[i].gameObject.GetComponent<Renderer>();
                Color currColor = rend.material.color;

                if (lower) currColor.a = reducedAlpha;
                else currColor.a = maxAlpha;

                rend.material.color = currColor;
            }
        }
    }
    // For Graphs X Y Z, Check if there is more than 1 line (Normal distribution), if true it means there is a point to clear
    private void CheckClearLastLine(DynamicGraph graph)
    {
        if (graph.GetLinesCount() > 1)
        {
            graph.ClearLastLine();
        }
    }

    private void DrawNormalCurve(DynamicGraph graph, Color color, float mean, float sigma)
    {
        if (!graph) return;

        Vector2 startEndXCoord = graph.GetXRange();
        int[] indices = GetSortedIndices();
        float meanSpeed = mean;
        float sigmaSpeed = sigma;

        graph.CreateLine(color, false, "Normal distribution");

        for (float x = startEndXCoord.x; x < startEndXCoord.y; x += 0.1f)
        {
            float yNormal = NormalDistribution.NormalPDF(x, sigmaSpeed, meanSpeed) * sim.GetNumBodies();
            Vector2 newPos = new Vector2(x, yNormal);
            graph.PlotPointOnLastLine(newPos);
        }
    }

    private void DrawStandardDeviation(DynamicGraph graph, Color color, float sigma)
    {
        if (!graph) return;

        graph.CreateLine(color, false, "Standard deviation");

        float meanSpeed = sim.GetMeanSpeed();
        
        float yStd = NormalDistribution.NormalPDF(sigma, sigma, meanSpeed) * sim.GetNumBodies();

        Vector2 startEndXCoord = new Vector2(meanSpeed, sigma);
            
        graph.PlotPointOnLastLine(new Vector2(startEndXCoord.x, yStd));
        graph.PlotPointOnLastLine(new Vector2(startEndXCoord.y, yStd));
    }

    // Can be use to debug the histogram graph
    private void PrintListVelocityCount()
    {
        string text = "";
        listVelocityAndCount.ForEach((vec) => text+=vec+", ");
        print(text);
    }

    private int GetMaxCountVelocityList()
    {
        int maxCount = 0;
        foreach(Vector2 vec in listVelocityAndCount)
        {
            if (maxCount < (int) vec.y)
            {
                maxCount = (int) vec.y;
            }
        }

        return maxCount;
    }

    private void InitializeVelocityList(DynamicGraph graph, float interval)
    {
        Vector2 xRange = graph.GetXRange();

        for (float i = xRange.x; i <= xRange.y; i += interval)
        {
            listVelocityAndCount.Add(Vector2.right * i);
        }
    }

    // Initialize blocks AFTER Velocity List !
    private void InitializeBlocks(DynamicGraph graph)
    {
        for (int i = 0; i < listVelocityAndCount.Count; i++)
        {
            graph.CreateBlock(listVelocityAndCount[i], "Block "+i);
        }
    }

    private void CheckContainsVelocityAndIncrease(float velocity)
    {
        // .5 Round 
        //float newVelocity = Mathf.Round(2*velocity)/2;

        // 1 Round
        float newVelocity = Mathf.Round(velocity);

        Vector2 vectorToUpdate = listVelocityAndCount.Find(vec => vec.x == newVelocity);

        if((vectorToUpdate != Vector2.zero) || (newVelocity == 0 && vectorToUpdate == Vector2.zero))
        {
            listVelocityAndCount.Remove(vectorToUpdate);
            listVelocityAndCount.Add(vectorToUpdate + Vector2.up);
        } else
        {
            listVelocityAndCount.Add(new Vector2(newVelocity, 1));
        }

        // Sort the list 
        listVelocityAndCount.Sort((vec1, vec2) => vec1.x.CompareTo(vec2.x));
    }

    private IEnumerator LoopOverBodiesMeasureRadialVelocity(float maxValue)
    {
        int[] indices = GetSortedIndices();
        float currentRadialVelocity = 0;
        float currentSumRadialVelocity = 0;

        TextMeshProUGUI counter = default;
        TextMeshProUGUI value = default;

        if (panelRadialVelocity)
        {
            panelRadialVelocity.gameObject.SetActive(true);
            Transform counterRadialVelocity = panelRadialVelocity.Find("Counter");
            if (counterRadialVelocity)
            {
                counterRadialVelocity.TryGetComponent(out counter);
            }
            Transform valueRadialVelocity = panelRadialVelocity.Find("Value");
            if (valueRadialVelocity)
            {
                valueRadialVelocity.TryGetComponent(out value);
            }
        }

        // Reset the graph and the list of count before plotting the graph
        if (graph) 
        {
            graph.Clear();
            listVelocityAndCount.Clear();
            InitializeVelocityList(graph, 1f);
            InitializeBlocks(graph);
        } 

        // Highlight the bodies one-by-one and show velocities
        for (int i = 0; i < indices.Length; i++)
        {
            Transform body = prefabs.bodies[indices[i]];
            Vector3 velocity = sim.GetVelocity(indices[i]);

            // The radial velocity is the velocity on the Z axis
            currentRadialVelocity = velocity.z;
            CheckContainsVelocityAndIncrease(currentRadialVelocity);
            // Plot the graph
            if (graph)
            {
                // Plot the point on the graph
                /*
                graph.CreateLine(Color.blue, true, "Point "+i);
                graph.PlotPoint(i, Vector2.right * currentRadialVelocity);
                */

                // .5 Round 
                //float newVelocity = Mathf.Round(2*currentRadialVelocity)/2;
                // 1 Round
                float newVelocity = Mathf.Round(currentRadialVelocity);

                Vector2 currBlock = listVelocityAndCount.Find(vec => vec.x == newVelocity);

                // Draw the current block on the graph
                graph.CreateBlock(currBlock, "Block "+i);

                // Draw the line on the exterior parts of the blocks
                graph.DrawExteriorBorder(listVelocityAndCount, colorBorders);
            }

            currentSumRadialVelocity += Mathf.Abs(currentRadialVelocity);
            if (counter)
            {
                counter.text = (i + 1).ToString();
            }
            if (value)
            {
                value.text = currentRadialVelocity.ToString("0.00");
            }

            // Update Radial Velocity
            UpdateRadialVelocityMeter(currentSumRadialVelocity / maxValue);

            // Highlight the current body
            body.GetComponent<MeshRenderer>().material = glowMaterial;

            // Show appropriate index label
            if (bodyLabels.Count > i && prefabs.BodiesHaveLabels())
            {
                Transform label = body.Find("Label");
                if (label)
                {
                    label.GetComponent<SpriteRenderer>().sprite = bodyLabels[i];
                    label.gameObject.SetActive(true);
                }
            }

            // Draw the body's velocity vector
            if (velocityPrefab)
            {
                velocityArrow = Instantiate(velocityPrefab, Vector3.zero, Quaternion.identity, simulation.transform).GetComponent<Arrow>();
                velocityArrow.transform.position = body.position;
                velocityArrow.SetComponents(new Vector3(0, 0, velocity.z) * lengthVectors, true);
            }

            yield return new WaitForSeconds(waitTimeIteration);

            // Destroy the velocity vector
            if (velocityArrow)
            {
                Destroy(velocityArrow.gameObject);
                velocityArrow = null;
            }

            HideBodyLabels();
        }

        // Uncomment if it is better with no Button
        yield return new WaitForSeconds(waitTimeBeforePlotNormalGraph);

        // Uncomment if it is better with no Button
        // Animation Plot Normal distribution
        //StartCoroutine(AnimationNormalDistribution(animationDuration));

        ResetBodyMaterials();
        HideBodyLabels();
        HideTextPanels();
        SetButtonsInteractivity(true, true);
    }

    private IEnumerator AnimationNormalDistribution(float animationDuration)
    {
        if (!graph) yield return null;

        float time = 0;
        Vector2 startEndXCoord = graph.GetXRange();
        int[] indices = GetSortedIndices();

        graph.CreateLine(colorNormalFit, false, "Normal distribution");

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            t = t * t * (3f - 2f * t);  // Apply some smoothing

            float x = Mathf.Lerp(startEndXCoord.x, startEndXCoord.y, t);

            // Plot the normal distribution with the height as high as the highest
            int maxHeight = GetMaxCountVelocityList();
            int minHeight = 0;
            float maxNormal = NormalDistribution.NormalPDF(sim.GetMeanSpeed(), sim.GetSpeedSigma(), sim.GetMeanSpeed());

            float yNormal = Mathf.Lerp(minHeight, maxHeight, NormalDistribution.NormalPDF(x, sim.GetSpeedSigma(), sim.GetMeanSpeed()) / maxNormal);
            
            Vector2 newPos = new Vector2(x, yNormal);
            
            graph.PlotPointOnLastLine(newPos);

            yield return null;
        }

        StartCoroutine(AnimationStandardDeviation(animationStandardDeviationDuration));

        SetButtonsInteractivity(true, false);
    }

    private IEnumerator AnimationStandardDeviation(float animationDuration)
    {
        if (!graph) yield return null;

        float time = 0;
        float meanSpeed = sim.GetMeanSpeed();
        float sigmaSpeed = sim.GetSpeedSigma();

        int[] indices = GetSortedIndices();

        graph.CreateLine(colorStandardDeviation, false, "Standard deviation");

        // Compute the normal distribution with the height as high as the highest
        int maxHeight = GetMaxCountVelocityList();
        int minHeight = 0;
        float maxNormal = NormalDistribution.NormalPDF(meanSpeed, sigmaSpeed, meanSpeed);

        // Compute the standard deviation
        float result = 0f;
        for (int i = 0; i < indices.Length; i++)
        {
            float radialVelocity = sim.GetVelocity(indices[i]).z;
            result += Mathf.Pow(radialVelocity - meanSpeed, 2);
        }

        float computedSigma = Mathf.Sqrt(result/indices.Length);
            
        // The higher is yNormal, the lower is the std
        float maxStd = sigmaSpeed + 0.5f;
        float minStd = sigmaSpeed - 0.5f;

        float maxHeightMinStd = NormalDistribution.NormalPDF(meanSpeed, minStd, meanSpeed) * sim.GetNumBodies();

        float yStd = Mathf.Lerp(minHeight, maxHeight, NormalDistribution.NormalPDF(computedSigma, sigmaSpeed, meanSpeed) / maxNormal);
        
        sim.lastComputedSigma = Mathf.Lerp(maxStd, minStd, maxHeight/maxHeightMinStd);

        Vector2 startEndXCoord = new Vector2(meanSpeed, computedSigma);

        // Update the std value displayed
        UpdateSigmaText(sigmaValue);

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            t = t * t * (3f - 2f * t);  // Apply some smoothing

            float x = Mathf.Lerp(startEndXCoord.x, startEndXCoord.y, t);

            Vector2 newPos = new Vector2(x, yStd);
            
            graph.PlotPointOnLastLine(newPos);

            yield return null;
        }
    }

    private int[] GetSortedIndices()
    {
        // Sort according to body x position on the screen
        float[] xPositions = new float[prefabs.bodies.Count];
        int[] indices = new int[xPositions.Length];
        for (int i = 0; i < xPositions.Length; i++)
        {
            xPositions[i] = mainCamera.WorldToViewportPoint(prefabs.bodies[i].transform.position).x;
            indices[i] = i;
        }
        System.Array.Sort(xPositions, indices);

        return indices;
    }

    private void SetButtonsInteractivity(bool interactive, bool fitNormalInteractive = false)
    {
        foreach (Button button in buttons)
        {
            if (button.Equals(fitNormalButton))
            {
                button.interactable = fitNormalInteractive;
                break;
            }
            button.interactable = interactive;
        }
    }

    private void SetBloomVisibility(bool visible) 
    {
        if (globalVolume != null)
        {
            globalVolume.gameObject.SetActive(visible);
        } else 
        {
            Debug.LogWarning("Bloom can't be activated/deactivated without globalVolume");
        }
    }

    private void ShowAllUI()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }

        foreach (RectTransform equation in equations)
        {
            equation.gameObject.SetActive(true);
        }

        SetSliderVisibility(true);
    }

    private void SetSliderVisibility(bool visible)
    {
        foreach (Slider slider in sliders)
        {
            slider.gameObject.SetActive(visible);
        }
    }

    private void SetRadialVelocityVisibility(bool visible)
    {
        if (measureRadialVelocityButton)
        {
            measureRadialVelocityButton.gameObject.SetActive(visible);
        }
        if (panelRadialVelocity)
        {
            panelRadialVelocity.gameObject.SetActive(!visible);
        }
    }

    private void HideTextPanels()
    {
        if (panelRadialVelocity)
        {
            panelRadialVelocity.gameObject.SetActive(false);
        }
    }

    private void ResetBodyMaterials()
    {
        if (defaultMaterial)
        {
            foreach (Transform body in prefabs.bodies)
            {
                if (body.TryGetComponent(out MeshRenderer renderer))
                {
                    renderer.material = defaultMaterial;
                }
            }
        }
    }

    private void HideBodyLabels()
    {
        if (prefabs.BodiesHaveLabels())
        {
            foreach (Transform body in prefabs.bodies)
            {
                Transform label = body.Find("Label");
                if (label)
                {
                    label.gameObject.SetActive(false);
                }
            }
        }
    }

    private void UpdateRadialVelocityMeter(float fillAmount)
    {
        if (meterRadialVelocity)
        {
            meterRadialVelocity.SetFillAmount(fillAmount);
        }
    }

    public void HandleCameraHasRotated()
    {
        if (handRotate)
        {
            handRotate.TriggerFadeOut();
        }
    }

    public void NewDeactivateSimulation()
    {
        base.DeactivateSimulation();
        if (TryGetComponent(out RadialVelocityAnimation radialVelocityAnimation))
        {
            radialVelocityAnimation.Reset();
        }
        SetBloomVisibility(false);
        ClearVelocitiesVectors();
    }

    public void UpdateMassText()
    {
        char power = ' ';
        if (massText)
        {
            string massTextPowerTen = sim.ComputeMassOfCluster().ToString("0.0E0");
            massTextPowerTen = massTextPowerTen.Replace("E", " * 10");

            power = massTextPowerTen[massTextPowerTen.Length-1];
            massText.text = massTextPowerTen.Remove(massTextPowerTen.Length-1);
        }
        if (solarMassImage)
        {
            solarMassImage.UpdateUnitPosition();
        }
        if (powerText)
        {
            powerText.SetTextPower(power.ToString());
            powerText.UpdateUnitPosition();
        }
    }

    public void UpdateSigmaText(TextMeshProUGUI sigmaText, float customValue = 0f, bool useCustomValue = false)
    {
        float newSigma;
        if (useCustomValue)
        {
            newSigma = customValue * 10f;
        } else
        {
            newSigma = sim.lastComputedSigma * 10f;
        }
        sigmaText.text = newSigma.ToString("0.0")+" km/s";
    }
}
