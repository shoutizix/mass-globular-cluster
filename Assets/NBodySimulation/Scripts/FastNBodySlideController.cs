using System.Collections;
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
    [SerializeField] private RectTransform equationK;
    [SerializeField] private RectTransform panelK;
    [SerializeField] private MeterFill meterK;
    [SerializeField] private RectTransform equationU;
    [SerializeField] private RectTransform panelU;
    [SerializeField] private MeterFill meterU;
    [SerializeField] private RectTransform dataPanel;
    [SerializeField] private FadeOutUI handRotate;

    [Header("Buttons")]
    [SerializeField] private Button measureRadialVelocityButton;
    [SerializeField] private Button computeKButton;
    [SerializeField] private Button computeUButton;
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

    [Header("Body Connector")]
    [SerializeField] private GameObject connectorPrefab;
    [SerializeField] private float lineWidth = 0.1f;

    [Header("Vectors")]
    [SerializeField] private GameObject velocityPrefab;
    [SerializeField] private float lengthVectors = 5;

    [Header("Body Labels")]
    [SerializeField] private List<Sprite> bodyLabels;

    [Header("Graphs")]
    [SerializeField] private DynamicGraph graph;
    [SerializeField] private DynamicGraph graphX;
    [SerializeField] private bool normalDistributionOnGraphX = false;
    [SerializeField] private DynamicGraph graphY;
    [SerializeField] private bool normalDistributionOnGraphY = false;
    [SerializeField] private DynamicGraph graphZ;
    [SerializeField] private bool normalDistributionOnGraphZ = false;
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

    // Data Panel values
    private TextMeshProUGUI dataPanelU;
    private TextMeshProUGUI dataPanelK;
    private TextMeshProUGUI dataPanelE;
    private TextMeshProUGUI dataPanelV;

    // Mesure of the radial velocity
    private List<Vector2> listVelocityAndCount = new List<Vector2>();


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
        if (computeKButton)
        {
            buttons.Add(computeKButton);
        }
        if (computeUButton)
        {
            buttons.Add(computeUButton);
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
        if (equationK)
        {
            equations.Add(equationK);
        }
        if (equationU)
        {
            equations.Add(equationU);
        }

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

        // Get data panel references
        if (dataPanel)
        {
            dataPanelU = dataPanel.Find("U Value").GetComponent<TextMeshProUGUI>();
            dataPanelK = dataPanel.Find("K Value").GetComponent<TextMeshProUGUI>();
            dataPanelE = dataPanel.Find("E Value").GetComponent<TextMeshProUGUI>();
            dataPanelV = dataPanel.Find("V Value").GetComponent<TextMeshProUGUI>();
        }
    }

    public override void InitializeSlide()
    {
        if (!prefabs)
        {
            return;
        }

        //Debug.Log(transform.name + " resetting " + sim.name);
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
        SetDataPanelVisibility(autoPlay);
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
            SetDataPanelVisibility(false);
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
        if (normalDistributionOnGraphX)
        {
            graphX.Clear();
            DrawNormalCurve(graphX, colorNormalFit);
        }

        if (normalDistributionOnGraphY)
        {
            graphY.Clear();
            DrawNormalCurve(graphY, colorNormalFit);
        }

        if (normalDistributionOnGraphZ)
        {
            graphZ.Clear();
            DrawNormalCurve(graphZ,colorNormalFit);
        }
    }

    public void FitNormalDistributionVisually()
    {
        SetUVisibility(false);
        SetKVisibility(false);
        SetButtonsInteractivity(false);
        simulation.Pause();
        StartCoroutine(AnimationNormalDistribution(animationNormalDuration));
    }

    public void ComputeRadialVelocityVisually()
    {
        sim.CustomReset(false, true, false);
        UpdateRadialVelocityMeter(0);
        SetUVisibility(false);
        SetKVisibility(false);
        SetButtonsInteractivity(false);
        simulation.Pause();
        sim.ComputeRadialVelocity();
        StartCoroutine(LoopOverBodiesMeasureRadialVelocity(sim.RadialVelocity));
    }

    public void ComputeKVisually()
    {
        UpdateKMeter(0);
        SetUVisibility(false);
        SetRadialVelocityVisibility(false);
        SetButtonsInteractivity(false);
        simulation.Pause();
        sim.ComputeKineticEnergy();
        StartCoroutine(LoopOverBodies(sim.K));
    }

    public void ComputeUVisually()
    {
        UpdateUMeter(0);
        SetKVisibility(false);
        SetRadialVelocityVisibility(false);
        SetButtonsInteractivity(false);
        simulation.Pause();
        sim.ComputePotentialEnergy();
        StartCoroutine(LoopOverBodiesWithConnections(sim.U));
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
        CheckClearLastLine(graphX);
        CheckClearLastLine(graphY);
        CheckClearLastLine(graphZ);

        // Display the velocity along each axis on the corresponding graph
        graphX.CreateLine(Color.red, true, "");
        graphX.PlotPointOnLastLine(Vector2.right * velocity.x);

        graphY.CreateLine(Color.green, true, "");
        graphY.PlotPointOnLastLine(Vector2.right * velocity.y);

        graphZ.CreateLine(Color.blue, true, "");
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

    private void DrawNormalCurve(DynamicGraph graph, Color color)
    {
        if (!graph) return;

        Vector2 startEndXCoord = graph.GetXRange();
        int[] indices = GetSortedIndices();
        float meanSpeed = 0f;
        float sigmaSpeed = 2;

        graph.CreateLine(color, false, "Normal distribution");

        for (float x = startEndXCoord.x; x < startEndXCoord.y; x += 0.2f)
        {
            int maxHeight = 20;
            int minHeight = 0;
            float maxNormal = NormalDistribution.NormalPDF(meanSpeed, sigmaSpeed, meanSpeed);
            float yNormal = Mathf.Lerp(minHeight, maxHeight, NormalDistribution.NormalPDF(x, sigmaSpeed, meanSpeed) / maxNormal);
            
            Vector2 newPos = new Vector2(x, yNormal);
            graph.PlotPointOnLastLine(newPos);
        }
    }

/*
    private void DrawStandardDeviation(DynamicGraph graph, Color color)
    {
        if (!graph) return;

        float startXCoord = -5f;
        float endXCoord = 5f;
        int[] indices = GetSortedIndices();
        float meanSpeed = 0f;
        float sigmaSpeed = 2;
        float maxNormal = NormalDistribution.NormalPDF(meanSpeed, sigmaSpeed, meanSpeed);

        graph.CreateLine(color, false, "Normal distribution");
        graph.PlotPointOnLastLine(Vector2.up * maxNormal);
        graph.PlotPointOnLastLine(new Vector2(sigmaSpeed, maxNormal));
    }
*/

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
        SetUVisibility(true);
        SetKVisibility(true);
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

    private IEnumerator LoopOverBodies(float maxValue)
    {
        int[] indices = GetSortedIndices();
        float currentK = 0;

        TextMeshProUGUI counter = default;
        TextMeshProUGUI value = default;

        if (panelK)
        {
            Transform counterK = panelK.Find("Counter");
            if (counterK)
            {
                counterK.TryGetComponent(out counter);
            }
            Transform valueK = panelK.Find("Value");
            if (valueK)
            {
                valueK.TryGetComponent(out value);
            }
        }

        // Highlight the bodies one-by-one and show velocities
        for (int i = 0; i < indices.Length; i++)
        {
            Transform body = prefabs.bodies[indices[i]];
            Vector3 velocity = sim.GetVelocity(indices[i]);

            // Update running K
            currentK += 0.5f * sim.GetMass(indices[i]) * velocity.sqrMagnitude;
            if (counter)
            {
                counter.text = (i + 1).ToString();
            }
            if (value)
            {
                value.text = currentK.ToString("0.00");
            }

            UpdateKMeter(currentK / maxValue);

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
                velocityVector = Instantiate(velocityPrefab, Vector3.zero, Quaternion.identity, simulation.transform).GetComponent<Vector>();
                velocityVector.SetPositions(body.position, body.position + velocity);
                velocityVector.Redraw();
            }

            yield return new WaitForSeconds(0.4f);

            // Destroy the velocity vector
            if (velocityVector)
            {
                Destroy(velocityVector.gameObject);
                velocityVector = null;
            }

            HideBodyLabels();
        }

        yield return new WaitForSeconds(2);

        ResetBodyMaterials();
        HideBodyLabels();
        SetButtonsInteractivity(true);
        SetUVisibility(true);
        SetRadialVelocityVisibility(true);
        simulation.Resume();
    }

    private IEnumerator LoopOverBodiesWithConnections(float maxValue)
    {
        int[] indices = GetSortedIndices();
        float currentU = 0;

        TextMeshProUGUI counter = default;
        TextMeshProUGUI value = default;

        if (panelU)
        {
            Transform counterU = panelU.Find("Counter");
            if (counterU)
            {
                counterU.TryGetComponent(out counter);
            }
            Transform valueU = panelU.Find("Value");
            if (valueU)
            {
                valueU.TryGetComponent(out value);
            }
        }

        // Highlight the bodies and their connections one-by-one
        for (int i = 0; i < indices.Length - 1; i++)
        {
            Transform body1 = prefabs.bodies[indices[i]];

            if (counter)
            {
                counter.text = (i + 1).ToString();
            }

            // Highlight the current body
            body1.GetComponent<MeshRenderer>().material = glowMaterial;

            // Show appropriate index label
            if (bodyLabels.Count > i && prefabs.BodiesHaveLabels())
            {
                Transform label = body1.Find("Label");
                if (label)
                {
                    label.GetComponent<SpriteRenderer>().sprite = bodyLabels[i];
                    label.gameObject.SetActive(true);
                }
            }

            // Connect the current body with remaining others
            connectors = new HashSet<GameObject>();
            for (int j = i + 1; j < indices.Length; j++)
            {
                Transform body2 = prefabs.bodies[indices[j]];

                currentU += sim.GravitationalPotentialEnergy(i, j);

                LineRenderer connector = Instantiate(connectorPrefab, Vector3.zero, Quaternion.identity, simulation.transform).GetComponent<LineRenderer>();
                connector.startWidth = lineWidth;
                connector.endWidth = lineWidth;
                connector.positionCount = 2;
                connector.SetPositions(new Vector3[] { body1.position, body2.position });
                connectors.Add(connector.gameObject);
            }

            // Update running U
            if (value)
            {
                value.text = currentU.ToString("0.00");
            }

            UpdateUMeter(currentU / maxValue);

            yield return new WaitForSeconds(0.4f);

            foreach (GameObject connector in connectors)
            {
                Destroy(connector);
            }

            HideBodyLabels();
        }

        yield return new WaitForSeconds(2);

        ResetBodyMaterials();
        SetButtonsInteractivity(true);
        SetKVisibility(true);
        SetRadialVelocityVisibility(true);
        simulation.Resume();
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

    private void SetUVisibility(bool visible)
    {
        if (equationU)
        {
            equationU.gameObject.SetActive(visible);
        }
        if (computeUButton)
        {
            computeUButton.gameObject.SetActive(visible);
        }
        if (panelK)
        {
            panelK.gameObject.SetActive(!visible);
        }
    }

    private void SetKVisibility(bool visible)
    {
        if (equationK)
        {
            equationK.gameObject.SetActive(visible);
        }
        if (computeKButton)
        {
            computeKButton.gameObject.SetActive(visible);
        }
        if (panelU)
        {
            panelU.gameObject.SetActive(!visible);
        }
    }

    private void HideTextPanels()
    {
        if (panelRadialVelocity)
        {
            panelRadialVelocity.gameObject.SetActive(false);
        }
        if (panelK)
        {
            panelK.gameObject.SetActive(false);
        }
        if (panelU)
        {
            panelU.gameObject.SetActive(false);
        }
    }

    private void SetDataPanelVisibility(bool visible)
    {
        if (dataPanel)
        {
            dataPanel.gameObject.SetActive(visible);
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

    // Called by StartButton
    public void TogglePlayPause()
    {
        if (sim.paused)
        {
            sim.Resume();
            SetSliderVisibility(false);
            SetDataPanelVisibility(true);
            foreach (Transform child in startButton.transform)
            {
                child.gameObject.SetActive(child.name == "Pause Text");
            }
        }
        else
        {
            sim.Pause();
            //SetSliderVisibility(true);
            //SetDataPanelVisibility(false);
            foreach (Transform child in startButton.transform)
            {
                child.gameObject.SetActive(child.name == "Resume Text");
            }
        }
    }

    public void UpdateDataPanel()
    {
        if (dataPanelU)
        {
            dataPanelU.text = (sim.U / sim.E).ToString("0.0");
        }
        if (dataPanelK)
        {
            dataPanelK.text = (sim.K / sim.E).ToString("0.0");
        }
        if (dataPanelE)
        {
            dataPanelE.text = ((sim.U + sim.K) / sim.E).ToString("0.0");
        }
        if (dataPanelV)
        {
            dataPanelV.text = (sim.averageVirial / sim.E).ToString("0.0");
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

    private void UpdateKMeter(float fillAmount)
    {
        if (meterK)
        {
            meterK.SetFillAmount(fillAmount);
        }
    }

    private void UpdateUMeter(float fillAmount)
    {
        if (meterU)
        {
            meterU.SetFillAmount(fillAmount);
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
