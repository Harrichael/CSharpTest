using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Bb;
using Point;

public static class Solver<T>
{
    // -----------------------------------
    // Private Data
    // -----------------------------------
    private Bb Map;


    // -----------------------------------
    // Constructor, persisting state for instance methods
    // -----------------------------------
    public Solver(ref Bb map)
    {
        this.map = map;
    }


    // -----------------------------------
    // Static Methods
    // -----------------------------------


    //<summary>
    // This function splits sources and targets into different possible solutions
    // Each solution is a different potential outcome, split returns list of solutions
    // The solution that maximizes the most is chosen to be acted upon
    // Returns optimal solution
    //</summary>
    public static
    IEnumerable< Tuple< IEnumerable<T> sources, IEnumerable<T> targets > >
    SplitMaxAct
        (
        IEnumerable<T> sources, 
        IEnumerable<T> targets,
        Func<
            IEnumerable<T> sources, 
            IEnumerable<T> targets,
            IEnumerable< IEnumerable< Tuple< IEnumerable<T> sources, IEnumerable<T> targets > > >
            > split,
        Func<
            IEnumerable< Tuple< IEnumerable<T> sources, IEnumerable<T> targets > >,
            int
            > maximize,
        Action<
            IEnumerable<T> sources,
            IEnumerable<T> targets
            > action
        )
    {
        var optSol = split( sources, targets ).Max( solution => maximize(solution) );
        optSol.Select( pair => action( pair.Item1, pair.Item2 ) );
        return optSol;
    }


    //<summary>
    // This function maps a set of sources to a set of targets
    // on a 1-1 basis so that termination is when
    // sources or targets are exhausted
    // returns all solutions with 1 source to 1 target
    // essentially cartesian product but wrapped in enumerables
    //</summary>
    public static IEnumerable< IEnumerable< Tuple< IEnumerable<T>, IEnumerable<T> > > >
    SinglePairSplit
        (
        IEnumerable<T> sources, 
        IEnumerable<T> targets
        )
    {
        foreach (var src in sources)
        {
            foreach (var targ in targets)
            {
                yield return new List< Tuple< IEnumerable<T>, IEnumerable<T> > >()
                {
                    Tuple.create(src, targ);
                }
            }
        }
    }


    //<summary>
    // This function maps a set of sources to a set of targets
    // on a 1-1 basis so that termination is when
    // sources or targets are exhausted
    // returns all possible solutions
    //</summary>
    public static IEnumerable< IEnumerable< Tuple< IEnumerable<T>, IEnumerable<T> > > >
    GroupPairSplit
        (
        IEnumerable<T> sources, 
        IEnumerable<T> targets
        )
    {
        // TODO
        // need permutations and zip
        // if sources is greater than targets, then problems
        // python: return map( lambda tper: zip(sources, tper), permutations(targets) );
        return targets.permutations().Select( tper => sources.zip(tper) );
    }


    //<summary>
    // This function splits srcs and targs in all solution pairings
    // such that 1-1 pairs are made with no duplication
    // The solution that maximizes the most is chosen to be acted upon
    // Returns optimal solution
    //</summary>
    public static
    IEnumerable< Tuple< IEnumerable<T> sources, IEnumerable<T> targets > >
    GroupPairMaxAct
        (
        IEnumerable<T> sources, 
        IEnumerable<T> targets,
        Func<
            IEnumerable< Tuple< IEnumerable<T> sources, IEnumerable<T> targets > >,
            int
            > maximize,
        Action<
            IEnumerable<T> sources,
            IEnumerable<T> targets
            > action
        )
    {
        return SplitMaxAct
            (
            sources,
            targets,
            Solver.GroupPairSplit,
            maximize,
            action
            );
    }


    public Point FastestSpawnAct
        (
        Func<Point, bool> isSpawnable,
        IEnumerable<Point> targets,
        int moveSpeed,
        Action<
            IEnumerable<T> sources,
            IEnumerable<T> targets
            > action
        )
    {
        var search = new Pather.Search(targets, isPassable, p => false);
        var sources = search.GScore.Keys.Where(s => isSpawnable(s));
        if (!reachable.Any())
        {
            return new Point(-1, -1);
        }

        Func< IEnumerable< Tuple< IEnumerable<T>, IEnumerable<T> > >, int > shortestTimeTravel = (solution) =>
        {
            var s = solution.first();
            return -( search.GScore[s] + map.GetSpawnDelay(s) * moveSpeed);
        };

        return SplitMaxAct
            (
            sources,
            targets,
            SinglePairSplit
            shortestTimeTravel,
            action
            ).first().Item1;
    }
    
    // -----------------------------------
    // Instance Methods
    // -----------------------------------

}
