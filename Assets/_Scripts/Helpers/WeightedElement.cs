using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedElement<T> : IComparable<WeightedElement<T>>
{
    public T Value { get; set; }
    public int Weight { get; set; }

    public WeightedElement(T value, int weight)
    {
        Value = value;
        Weight = weight;
    }

    public int CompareTo(WeightedElement<T> other)
    {
        if (other == null) return 1;
        return Weight.CompareTo(other.Weight);
    }

    public override string ToString()
    {
        return $"WeightedElement (Value: {Value}, Weight: {Weight})";
    }
}
