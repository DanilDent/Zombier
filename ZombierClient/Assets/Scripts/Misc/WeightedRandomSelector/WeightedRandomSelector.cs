using System;
using System.Collections.Generic;

namespace Prototype.Misc
{
    public class WeightedRandomSelector<T>
        where T : class, IWeighted
    {
        private List<T> elements;
        private List<float> cumulativeWeights;
        private float totalWeight;
        private Random random;

        public WeightedRandomSelector(List<T> elements)
        {
            this.elements = elements;
            cumulativeWeights = new List<float>(elements.Count);
            totalWeight = 0f;

            // Calculate the cumulative weights based on the given elements' weights
            foreach (var element in elements)
            {
                float weight = GetWeight(element);
                totalWeight += weight;
                cumulativeWeights.Add(totalWeight);
            }

            random = new Random();
        }

        public T GetRandomElement()
        {
            if (elements.Count == 0)
            {
                throw new InvalidOperationException("No records in the pool.");
            }

            float randomValue = (float)random.NextDouble() * totalWeight;

            // Binary search to find the index of the selected record in cumulativeWeights
            int index = cumulativeWeights.BinarySearch(randomValue);
            if (index < 0)
            {
                index = ~index;
            }

            if (index >= elements.Count)
            {
                index = elements.Count - 1;
            }

            return elements[index];
        }

        private float GetWeight(T element)
        {
            return (element as IWeighted).Weight;
        }
    }
}
