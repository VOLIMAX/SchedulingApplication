using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;

namespace SchedulingApplication.Services
{
    public class SolutionPrinter : CpSolverSolutionCallback, ISolutionPrinter
    {
        private int solutionCount_;
        private readonly int[] _allGuards;
        private readonly int[] allDays_;
        private readonly int[] allShifts_;
        private readonly Dictionary<Tuple<int, int, int>, IntVar> shifts_;
        private readonly int solutionLimit_;
        public SolutionPrinter(int[] allGuards, int[] allDays, int[] allShifts,
                               Dictionary<Tuple<int, int, int>, IntVar> shifts, int limit)
        {
            solutionCount_ = 0;
            _allGuards = allGuards;
            allDays_ = allDays;
            allShifts_ = allShifts;
            shifts_ = shifts;
            solutionLimit_ = limit;
        }

        public override void OnSolutionCallback()
        {
            Console.WriteLine($"Solution #{solutionCount_}:");
            foreach (int d in allDays_)
            {
                Console.WriteLine($"Day {d}");
                foreach (int n in _allGuards)
                {
                    bool isWorking = false;
                    foreach (int s in allShifts_)
                    {
                        var key = Tuple.Create(n, d, s);
                        if (Value(shifts_[key]) == 1L)
                        {
                            isWorking = true;
                            Console.WriteLine($"  Guard {n} work shift {s}");
                        }
                    }
                    if (!isWorking)
                    {
                        Console.WriteLine($"  Guard {d} does not work");
                    }
                }
            }
            solutionCount_++;
            if (solutionCount_ >= solutionLimit_)
            {
                Console.WriteLine($"Stop search after {solutionLimit_} solutions");
                StopSearch();
            }
        }

        public IEnumerable<object> CalculateSolutions(int guardsNumber, int daysNumber, int shiftsNumber)
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
            // shifts[(n, d, s)]: nurse 'n' works shift 's' on day 'd'.
            Dictionary<Tuple<int, int, int>, IntVar> shifts = new();
            foreach (int n in allGuards)
            {
                foreach (int d in allDays)
                {
                    foreach (int s in allShifts)
                    {
                        shifts.Add(Tuple.Create(n, d, s), model.NewBoolVar($"shifts_n{n}d{d}s{s}"));
                    }
                }
            }

            // Each shift is assigned to exactly one nurse in the schedule period.
            foreach (int d in allDays)
            {
                foreach (int s in allShifts)
                {
                    IntVar[] x = new IntVar[_guardsNumber];
                    foreach (int n in allGuards)
                    {
                        var key = Tuple.Create(n, d, s);
                        x[n] = shifts[key];
                    }
                    model.Add(LinearExpr.Sum(x) == 1);
                }
            }

            // Each guard works at most one shift per day.
            foreach (int n in allGuards)
            {
                foreach (int d in allDays)
                {
                    IntVar[] x = new IntVar[_shiftsNumber];
                    foreach (int s in allShifts)
                    {
                        var key = Tuple.Create(n, d, s);
                        x[s] = shifts[key];
                    }
                    model.Add(LinearExpr.Sum(x) <= 1);
                }
            }

            // Try to distribute the shifts evenly, so that each nurse works
            // minShiftsPerNurse shifts. If this is not possible, because the total
            // number of shifts is not divisible by the number of nurses, some nurses will
            // be assigned one more shift.
            int minShiftsPerNurse = (_shiftsNumber * _daysNumber) / _guardsNumber;
            int maxShiftsPerNurse;
            if ((_shiftsNumber * _daysNumber) % _guardsNumber == 0)
            {
                maxShiftsPerNurse = minShiftsPerNurse;
            }
            else
            {
                maxShiftsPerNurse = minShiftsPerNurse + 1;
            }
            foreach (int n in allGuards)
            {
                IntVar[] numShiftsWorked = new IntVar[_daysNumber * _shiftsNumber];
                foreach (int d in allDays)
                {
                    foreach (int s in allShifts)
                    {
                        var key = Tuple.Create(n, d, s);
                        numShiftsWorked[d * _shiftsNumber + s] = shifts[key];
                    }
                }
                model.AddLinearConstraint(LinearExpr.Sum(numShiftsWorked), minShiftsPerNurse, maxShiftsPerNurse);
            }

            CpSolver solver = new CpSolver();
            solver.StringParameters += "linearization_level:0 ";
            // Tell the solver to enumerate all solutions.
            solver.StringParameters += "enumerate_all_solutions:true ";

            // Display the first five solutions.
            const int solutionLimit = 5;
            SolutionPrinter cb = new(allGuards, allDays, allShifts, shifts, solutionLimit);

            // Solve
            CpSolverStatus status = solver.Solve(model, cb);
            Console.WriteLine($"Solve status: {status}");

            Console.WriteLine("Statistics");
            Console.WriteLine($"  conflicts: {solver.NumConflicts()}");
            Console.WriteLine($"  branches : {solver.NumBranches()}");
            Console.WriteLine($"  wall time: {solver.WallTime()}s");

            return Array.Empty<object>();
        }
    }
}
