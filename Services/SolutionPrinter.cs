using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;
using SchedulingApplication.Models;

namespace SchedulingApplication.Services
{
    public class SolutionPrinter : CpSolverSolutionCallback, ISolutionPrinter
    {
        private int _solutionCount;
        private int[] _allGuards;
        private int[] _allDays;
        private int[] _allShifts;
        private Dictionary<Tuple<int, int, int>, IntVar> _shifts;
        private int _solutionLimit;
        public List<string> _solutionsInfo = new ();
        public List<string> _solutions = new ();

        public SolutionPrinter()
        {

        }

        public void SetData(int[] allGuards, int[] allDays, int[] allShifts,
                               Dictionary<Tuple<int, int, int>, IntVar> shifts, int limit)
        {
            _solutionCount = 0;
            _allGuards = allGuards;
            _allDays = allDays;
            _allShifts = allShifts;
            _shifts = shifts;
            _solutionLimit = limit;
        }

        public override void OnSolutionCallback()
        {            
            _solutions.Add($"Розв'язок #{_solutionCount + 1}:");
            foreach (int d in _allDays)
            {                
                _solutionsInfo.Add($"День {d + 1}");
                foreach (int g in _allGuards)
                {
                    bool isWorking = false;
                    foreach (int s in _allShifts)
                    {
                        var key = Tuple.Create(g, d, s);
                        if (Value(_shifts[key]) == 1L)
                        {
                            isWorking = true;                            
                            _solutionsInfo.Add($"Охоронець {g + 1} працює у {s + 1} зміну");
                        }
                    }
                    if (!isWorking)
                    {                        
                        _solutionsInfo.Add($"Охоронець {g + 1} не працює");
                    }
                }
                _solutionsInfo.Add($"----------------------------");
            }            
            _solutionCount++;
            if (_solutionCount >= _solutionLimit)
            {
                //TODO: implement logger
                //Console.WriteLine($"Stop search after {_solutionLimit} solutions");
                StopSearch();
            }
        }

        public SchedulingModel CalculateSolutions(int guardsNumber, int daysNumber, int shiftsNumber)
        {
            int _guardsNumber = guardsNumber;
            int _daysNumber = daysNumber;
            int _shiftsNumber = shiftsNumber;

            int[] allGuards = Enumerable.Range(0, _guardsNumber).ToArray();
            int[] allDays = Enumerable.Range(0, _daysNumber).ToArray();
            int[] allShifts = Enumerable.Range(0, _shiftsNumber).ToArray();

            // Creates the model.
            CpModel model = new();

            // Creates shift variables.
            // shifts[(g, d, s)]: guard 'g' works shift 's' on day 'd'.
            Dictionary<Tuple<int, int, int>, IntVar> shifts = new();
            foreach (int g in allGuards)
            {
                foreach (int d in allDays)
                {
                    foreach (int s in allShifts)
                    {
                        shifts.Add(Tuple.Create(g, d, s), model.NewBoolVar($"shifts_g{g}d{d}s{s}"));
                    }
                }
            }

            // Each shift is assigned to exactly one guard in the schedule period.
            foreach (int d in allDays)
            {
                foreach (int s in allShifts)
                {
                    IntVar[] x = new IntVar[_guardsNumber];
                    foreach (int g in allGuards)
                    {
                        var key = Tuple.Create(g, d, s);
                        x[g] = shifts[key];
                    }
                    model.Add(LinearExpr.Sum(x) == 1);
                }
            }

            // Each guard works at most one shift per day.
            foreach (int g in allGuards)
            {
                foreach (int d in allDays)
                {
                    IntVar[] x = new IntVar[_shiftsNumber];
                    foreach (int s in allShifts)
                    {
                        var key = Tuple.Create(g, d, s);
                        x[s] = shifts[key];
                    }
                    model.Add(LinearExpr.Sum(x) <= 1);
                }
            }

            // Try to distribute the shifts evenly, so that each guard works
            // minShiftsPerNurse shifts. If this is not possible, because the total
            // number of shifts is not divisible by the number of guards, some guards will
            // be assigned one more shift.
            int minShiftsPerGuard = (_shiftsNumber * _daysNumber) / _guardsNumber;
            int maxShiftsPerGuard;
            if ((_shiftsNumber * _daysNumber) % _guardsNumber == 0)
            {
                maxShiftsPerGuard = minShiftsPerGuard;
            }
            else
            {
                maxShiftsPerGuard = minShiftsPerGuard + 1;
            }
            foreach (int g in allGuards)
            {
                IntVar[] numShiftsWorked = new IntVar[_daysNumber * _shiftsNumber];
                foreach (int d in allDays)
                {
                    foreach (int s in allShifts)
                    {
                        var key = Tuple.Create(g, d, s);
                        numShiftsWorked[d * _shiftsNumber + s] = shifts[key];
                    }
                }
                model.AddLinearConstraint(LinearExpr.Sum(numShiftsWorked), minShiftsPerGuard, maxShiftsPerGuard);
            }

            CpSolver solver = new();
            solver.StringParameters += "linearization_level:0 ";
            // Tell the solver to enumerate all solutions.
            solver.StringParameters += "enumerate_all_solutions:true ";

            const int solutionLimit = 5;
            
            SolutionPrinter cb = this;
            SetData(allGuards, allDays, allShifts, shifts, solutionLimit);

            // Solve
            CpSolverStatus status = solver.Solve(model, cb);            

            int calculateElementsToFormAnObject = CalculateElementsToFormAnObject(_daysNumber, _guardsNumber);
            var solutionsInfoLists = SplitAllInfoIntoSolutionObjects(cb._solutionsInfo, calculateElementsToFormAnObject, solutionLimit);

            var statistics = new StatisticsModel
            {
                Conflicts = solver.NumConflicts(),
                Branches = solver.NumBranches(),
                WallTime = solver.WallTime(),
                SolverStatus = status
            };

            var data = new SchedulingModel 
            { 
                SolutionsInfoLists = solutionsInfoLists, 
                Statistics = statistics,
                Solutions = cb._solutions
            };

            return data;
        }

        private IList<List<string>> SplitAllInfoIntoSolutionObjects(List<string> solutionsInfo, int numElementsInAnObject, int solutionLimit)
        {
            IList<List<string>> solutionsInfoLists = new List<List<string>>();
            int itemsToSkip = 0;
            
            for (int i = 0; i < solutionLimit; i++)
            {
                var addObjectToTheMainList = solutionsInfo.Skip(itemsToSkip).Take(numElementsInAnObject).ToList();
                itemsToSkip += numElementsInAnObject;
                solutionsInfoLists.Add(addObjectToTheMainList);
            }
            
            return solutionsInfoLists;
        }

        private int CalculateElementsToFormAnObject(int numDays, int numGuards)
        {
            int numGuardsInAnObject = numDays * numGuards;            
            int calculateElementsToFormAnObject = numDays * 2 + numGuardsInAnObject; //double days number because we need also spacing

            return calculateElementsToFormAnObject;
        }
    }
}
