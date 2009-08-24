using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal delegate TReturn RecurseSeries<TReturn, TElement>(IEnumerable<TElement> series);
    internal delegate void RecurseStep<TElement>(TElement step);
    internal delegate void RecurseSeries<TElement>(IEnumerable<TElement> series);
    internal delegate TReturn RecurseTail<TReturn>();
    /* *
     * Used so when the visitor is seen, the store knows how to handle
     * it.  If it's a void recursor, and it expects a returner, 
     * an error is thrown.
     * */
    internal enum ExpectedVisitorType
    {
        Returner,
        Void,
    }

    internal interface IRecursionStore
    {
        R See<T, R>(ExpectedVisitorType type, IRecursionVisitor visitor, IEnumerable<T> series);
    }
    internal interface IRecursionVisitor
    {
        TReturn VisitReturner<TReturn, TElement>(IEnumerable<TElement> series, RecurseSeries<TReturn, TElement> callback);
        void VisitVoid<TElement>(IEnumerable<TElement> series, RecurseSeries<TElement> callback);
    }

    internal class RecursorStore<TElement> :
        IRecursionStore
    {
        RecurseSeries<TElement> stepper;

        public RecursorStore(RecurseSeries<TElement> stepper)
        {
            this.stepper = stepper;
        }

        #region IRecursionStore Members

        R IRecursionStore.See<T, R>(ExpectedVisitorType type, IRecursionVisitor visitor, IEnumerable<T> series)
        {
            switch (type)
            {
                case ExpectedVisitorType.Returner:
                    throw new InvalidOperationException("Expected void recursor type.");
                case ExpectedVisitorType.Void:
                    visitor.VisitVoid<TElement>((IEnumerable<TElement>)series, stepper);
                    break;
                default:
                    break;
            }
            return default(R);
        }

        #endregion
    }

    internal class RecursorStore<TElement, TReturn> :
        IRecursionStore
    {

        RecurseSeries<TReturn, TElement> stepper;

        public RecursorStore(RecurseSeries<TReturn, TElement> stepper)
        {
            this.stepper = stepper;
        }

        #region IRecursionStore Members

        R IRecursionStore.See<T, R>(ExpectedVisitorType type, IRecursionVisitor visitor, IEnumerable<T> series)
        {
            switch (type)
            {
                case ExpectedVisitorType.Returner:
                    return (R)(object)visitor.VisitReturner<TReturn, TElement>((IEnumerable<TElement>)series, stepper);
                case ExpectedVisitorType.Void:
                    throw new InvalidOperationException("Expected returner recursor type.");
                default:
                    break;
            }
            return default(R);
        }

        #endregion
    }

    internal class Recursor<TRecurseKey> :
        IRecursionVisitor
    {
        private Dictionary<TRecurseKey, IRecursionStore> recurseTracker = new Dictionary<TRecurseKey, IRecursionStore>();
        public Recursor() { }

        public void PrepareRecursion<TElement, TReturn>(TRecurseKey key, RecurseStep<TElement> stepwiseProcessor, RecurseTail<TReturn> returner)
        {
            if (recurseTracker.ContainsKey(key))
                throw new InvalidOperationException("A recursor under the key provided already exists.");
            IList<TElement> cyclicLock = new List<TElement>();
            int index = 0;
            RecurseSeries<TReturn, TElement> callback = (cSource) =>
            {
                return CyclicRecurseAid<TReturn, TElement>(cSource, stepwiseProcessor, returner, cyclicLock, ref index);
            };
            IRecursionStore store = new RecursorStore<TElement, TReturn>(callback);
            this.recurseTracker.Add(key, store);
        }

        public void PrepareRecursion<TElement>(TRecurseKey key, RecurseStep<TElement> stepwiseProcessor)
        {
            if (recurseTracker.ContainsKey(key))
                throw new InvalidOperationException("A recursor under the key provided already exists.");
            IList<TElement> cyclicLock = new List<TElement>();
            int index = 0;
            RecurseSeries<TElement> callback = (cSource) =>
            {
                CyclicRecurseAid<TElement>(cSource, stepwiseProcessor, cyclicLock, ref index);
            };
            IRecursionStore store = new RecursorStore<TElement>(callback);
            this.recurseTracker.Add(key, store);
        }

        private static TReturn CyclicRecurseAid<TReturn, T>(IEnumerable<T> source, RecurseStep<T> stepper, RecurseTail<TReturn> tail, IList<T> cyclicLock, ref int index)
        {
            foreach (var element in source)
            {
                if (cyclicLock.Contains(element))
                    continue;
                index++;
                cyclicLock.Add(element);
                stepper(element);
                index--;
            }
            if (index == 0)
            {
                cyclicLock.Clear();
                return tail();
            }
            return default(TReturn);
        }

        private static void CyclicRecurseAid<T>(IEnumerable<T> source, RecurseStep<T> stepper, IList<T> cyclicLock, ref int index)
        {
            foreach (var element in source)
            {
                if (cyclicLock.Contains(element))
                    continue;
                index++;
                cyclicLock.Add(element);
                stepper(element);
                index--;
            }
            if (index == 0)
                cyclicLock.Clear();
        }

        public TReturn Recurse<TReturn, TElement>(TRecurseKey key, IEnumerable<TElement> series)
        {
            if (!recurseTracker.ContainsKey(key))
                throw new ArgumentOutOfRangeException("key");
            return recurseTracker[key].See<TElement, TReturn>(ExpectedVisitorType.Returner, this, series);
        }

        public void Recurse<TElement>(TRecurseKey key, IEnumerable<TElement> series)
        {
            if (!recurseTracker.ContainsKey(key))
                throw new ArgumentOutOfRangeException("key");
            recurseTracker[key].See<TElement, int?>(ExpectedVisitorType.Void, this, series);
        }

        public TReturn RecurseSingle<TReturn, TElement>(TRecurseKey key, TElement single)
        {
            return this.Recurse<TReturn, TElement>(key, new TElement[] { single });
        }

        public void RecurseSingle<TElement>(TRecurseKey key, TElement single)
        {
            this.Recurse<TElement>(key, new TElement[] { single });
        }
        #region IRecursionVisitor Members

        TReturn IRecursionVisitor.VisitReturner<TReturn, TElement>(IEnumerable<TElement> series, RecurseSeries<TReturn, TElement> callback)
        {
            return callback(series);
        }

        void IRecursionVisitor.VisitVoid<TElement>(IEnumerable<TElement> series, RecurseSeries<TElement> callback)
        {
            callback(series);
        }

        #endregion
    }
}
