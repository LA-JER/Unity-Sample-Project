
using System.Collections.Generic;

public static class RandomSelect<T>
{
    public static List<T> ChooseRandomObjects(Dictionary<T, int> chanceTable, int count)
    {
        if (chanceTable != null && count > 0 && chanceTable.Count >= count)
        {
            var list = new List<T>();
            List<WeightedElement<T>> sortedWeightTable = new List<WeightedElement<T>>();
            //List<Node<T>> unsortedWeightTable = new List<Node<T>>();
            int sum = 0;

            foreach (var item in chanceTable)
            {
                sum += item.Value;
                sortedWeightTable.Add(new WeightedElement<T>(item.Key, sum));

            }

            sortedWeightTable.Sort();


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


}