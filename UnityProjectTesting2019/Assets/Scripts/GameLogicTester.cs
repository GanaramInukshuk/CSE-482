using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For testing out the scripts I make

public class GameLogicTester : MonoBehaviour {

    [SerializeField] [Range(0, (int)1e+4)]
    private int[] _housingToTest = new int[] { 6000, 1200, 600, 200, 100 };

    [SerializeField] [Range(-1, (int)1e+5)]
    private int _occupancyToTest = 10000;

    [SerializeField] [Range(0.1f, 10f)]
    private float[] _householdAffectors = new float[] { 2f, 1f, 2f, 3f, 1f, 1f };

    [SerializeField] [Range(10, 40)]
    private int[] _classroomSize = new int[] { 10, 20, 20, 20, 20 };

    [SerializeField] [Range(0, 40)]
    private int[] _schoolCount = new int[] { 1, 1, 1, 1, 1 };

    [SerializeField] [Range(0, 100)]
    private int[] _storesToTest = new int[] { 30, 30, 30, 30, 30, 30 };

    [SerializeField] [Range(0.1f, 10f)]
    private float[] _commerceAffectors = new float[] { 3f, 2f, 2f, 2f, 1f, };


    [SerializeField] [Range(0, 360*52*10)]
    private int _ticks = 69420;

    // Simulators to test; use this if simulators are regular classes
    //private ResidentialSimulator _resSim   = new ResidentialSimulator();
    //private CommercialSimulator  _commSim  = new CommercialSimulator ();
    //private WorkforceEvaluator   _workEval = new WorkforceEvaluator  ();

    // Simulators to test; use this if simulators derive from MonoBehaviour
    private ResidentialSimulator _resSim   ;
    private CommercialSimulator  _commSim  ;
    private WorkforceEvaluator   _workEval ;

    //private EducationScripts.SchoolManager _schoolMgr = new EducationScripts.SchoolManager();

    private Counter _timeCounter;

    private BasicManager _testMgr = new BasicManager(20);

    // Start is called before the first frame update
    void Start() {
        _timeCounter = new Counter();

        _resSim   = GetComponent<ResidentialSimulator>();
        _commSim  = GetComponent<CommercialSimulator >();
        _workEval = GetComponent<WorkforceEvaluator  >();

        string msg = 
            "Press ENTER to force GameLogicTester to load and use test data.\n" +
            "Press K to print debug string without loading test data."
            ;

        Debug.Log(msg);
    }

    // Debug stuff goes here; comment out when not needed
    private void Update() {

        if (Input.GetKeyDown(KeyCode.G)) {
            float[] random = new float[100];
            for (int i = 0; i < random.Length; i++) random[i] = ExtraRandom.RandomGauss(25000, (float)25000/3);
            Debug.Log(DistributionGen.Debug.ProbToString(random));
        }

        //if (Input.GetKeyDown(KeyCode.LeftBracket)) {
        //    _schoolMgr.IncrementSchools(-1);
        //    _schoolMgr.PrintDebugString();
        //}

        //if (Input.GetKeyDown(KeyCode.RightBracket)) {
        //    _schoolMgr.IncrementSchools(1);
        //    _schoolMgr.PrintDebugString();
        //}

        //if (Input.GetKeyDown(KeyCode.Comma)) {
        //    _schoolMgr.IncrementClassrooms(-1);
        //    _schoolMgr.PrintDebugString();
        //}

        //if (Input.GetKeyDown(KeyCode.Period)) {
        //    _schoolMgr.IncrementClassrooms(1);
        //    _schoolMgr.PrintDebugString();
        //}

        if (Input.GetKeyDown(KeyCode.Return)) {
            // For debugging, use the 4-param Generate function

            _resSim.Generate(_householdAffectors, _housingToTest, _occupancyToTest, 0);
            _resSim.PrintDebugString();

            _workEval.GenerateWorkforce(_resSim.PopulationBreakdown);
            _workEval.PrintDebugString();

            _commSim.Generate(_commerceAffectors, _storesToTest, _workEval.CommercialLabor, 0);
            _commSim.PrintDebugString();

            //int[] students = SimulatorAssistant.Education.K14Students(_resSim.PopulationBreakdown);
            //_eduSim.GenerateEducation(students, _schoolCount, _classroomSize);
            //_eduSim.PrintDebugString();

            //_clsMgr.Classrooms = 30;
            //_clsMgr.ClassroomSize = 24;
            //int students = _resSim.PopulationBreakdown.childPopulation;
            //_clsMgr.Generate(students);
            //_clsMgr.PrintDebugString();
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            // For general gameplay, use the 2-param Generate function if using affectors; otherwise,
            // use the 1-param Generate function, using the corresponding evaluator's increment amount

            _resSim.Generate();
            _resSim.PrintDebugString();

            _workEval.GenerateWorkforce(_resSim.PopulationBreakdown);
            _workEval.PrintDebugString();

            _commSim.Generate(_workEval.CommercialIncrement);
            _commSim.PrintDebugString();
        }

        //if (Input.GetKeyDown(KeyCode.T)) {
        //    //for (int i = 0; i < 300; i++) {
        //    //    _testMgr.Generate(i + 1, ResidentialScripts.Constants.DefaultPopulationWeights);
        //    //    _testMgr.PrintDebugString();
        //    //}
        //    _testMgr.Generate(10000, ResidentialScripts.Constants.DefaultPopulationWeights);
        //    _testMgr.PrintDebugString();
        //}

        // To test out saves
        if (Input.GetKeyDown(KeyCode.M)) {
            _resSim.DataVector = new int[][] {
                ExtraRandom.RandomArray( 7, 100, 1000),
                ExtraRandom.RandomArray( 2, 100, 1000),
                ExtraRandom.RandomArray( 2, 100, 1000),
                ExtraRandom.RandomArray(13, 100, 1000),
                ExtraRandom.RandomArray( 2, 100, 1000),
            };
            _resSim.PrintDebugString();
            _resSim.Generate();
            _resSim.PrintDebugString();
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            Debug.Log(Timekeeper.DetailedDate(_ticks));
        }
    }
}