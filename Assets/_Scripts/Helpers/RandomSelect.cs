
using System.Collections.Generic;
using System.Linq;
using System;
public static class RandomSelect<T>
{
    public static List<T> ChooseRandomObjects(Dictionary<T, int> chanceTable, int count)
    {
        if (chanceTable != null && count > 0 && chanceTable.Count >= count)
        {
            
            List<WeightedElement<T>> sortedWeightTable = new List<WeightedElement<T>>();
            //List<Node<T>> unsortedWeightTable = new List<Node<T>>();
            int sum = 0;

            foreach (var item in chanceTable)
            {
                sum += item.Value;
                sortedWeightTable.Add(new WeightedElement<T>(item.Key, sum));

            }

            sortedWeightTable.Sort();
            ShuffleElementsByWeight(sortedWeightTable);
            //Debugger.Log()

            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                int random = (int)(UnityEngine.Random.value * sum);
                for (int j = 0; j < sortedWeightTable.Count; j++)
                {


                    if (random <= sortedWeightTable[j].Weight)
                    {
                        list.Add(sortedWeightTable[j].Value);
                        break;
                    }
                }
            }

            return list;
        }
        else
        {
            return null;
        }

    }

    private static void ShuffleElementsByWeight<T>( List<WeightedElement<T>> list)
    {
        // Group elements by weight
        var groups = list.GroupBy(x => x.Weight).ToList();

        // Shuffle each group
        foreach (var group in groups)
        {
            var groupList = group.ToList();
            Shuffle(groupList);

            // Update the original list with shuffled group
            int index = list.FindIndex(x => x.Weight == group.Key);
            for (int i = 0; i < groupList.Count; i++)
            {
                list[index + i] = groupList[i];
            }
        }
    }
    private static void Shuffle<T>( IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = new Random().Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}