using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using GameItems;
using UnityEngine.SocialPlatforms;
using GameUI;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TreasureGame
{
    public class Entry : MonoBehaviour
    {
        public Inventory Inventory;
        public GameObject Answer;
        public Explosion Explosion;
        public ReadMap ReadMap;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) Explosion.Play(Explosion.Duration / 2);
            
            if (Input.GetKeyDown(KeyCode.I) && !Answer.activeSelf)
            {
                Inventory.Open();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {

            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if ((Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) && Camera.main.fieldOfView >= 6)
                {
                    Camera.main.fieldOfView -= 1;
                }
                if ((Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) && Camera.main.fieldOfView <= 39)
                {
                    Camera.main.fieldOfView += 1;
                }
            }
        }

        public void InitNonMonoBehaviours()
        {

        }
    }

    public class ObjectComparable
    {
        public static bool Compare(object obj1, object obj2, params string[] ignore)
        {
            if (obj1.GetType().Name != obj2.GetType().Name) return false;

            if (CompareProperties(obj1, obj2, ignore) && CompareFields(obj1, obj2, ignore)) return true;
            else return false;
        }

        private static bool CompareProperties(object obj1, object obj2, params string[] ignore)
        {
            PropertyInfo[] properties1 = obj1.GetType().GetProperties();
            PropertyInfo[] properties2 = obj2.GetType().GetProperties();

            foreach (PropertyInfo property1 in properties1)
            {
                if (ignore.FirstOrDefault(p => p == property1.Name) != null) continue;

                PropertyInfo property2 = properties2.FirstOrDefault(p => p.Name == property1.Name);
                if (property2 == null || property1.PropertyType != property2.PropertyType)
                    return false;

                if (!property1.GetValue(obj1).Equals(property2.GetValue(obj2))) return false;
            }
            return true;
        }

        private static bool CompareFields(object obj1, object obj2, params string[] ignore)
        {
            FieldInfo[] fields1 = obj1.GetType().GetFields();
            FieldInfo[] fields2 = obj2.GetType().GetFields();

            foreach (FieldInfo field1 in fields1)
            {
                if (ignore.FirstOrDefault(f => f == field1.Name) != null) continue;

                FieldInfo field2 = fields2.FirstOrDefault(f => f.Name == field1.Name);
                if (field2 == null || field1.FieldType != field2.FieldType)
                    return false;

                if (obj1 == null || obj2 == null) Debug.Log("Loi nullll");
                if (!field1.GetValue(obj1).Equals(field2.GetValue(obj2))) return false;
            }
            return true;
        }
    }

    public class ItemList : List<GameItem>
    {
        public delegate void Changed();
        private event Changed ListChanged;

        private bool IsRangeChangedProcess;

        public new void Add(GameItem newItem)
        {
            foreach (GameItem item in this)
                if (ObjectComparable.Compare(newItem, item, "Count"))
                {
                    item.Count += newItem.Count;
                    newItem = item;
                    return;
                }
            base.Add(newItem);

            if (!IsRangeChangedProcess) ListChanged?.Invoke();
        }

        public new void AddRange(IEnumerable<GameItem> newItems)
        {
            for (int i = 0; i < newItems.Count(); i++)
            {
                Add(newItems.ToArray()[i]);
                if (i != 0) IsRangeChangedProcess = true;
            }

            IsRangeChangedProcess = false;
        }

        public new void Remove(GameItem item)
        {
            base.Remove(item);
            ListChanged?.Invoke();
        }
        public new void Clear()
        {
            base.Clear();
            ListChanged?.Invoke();
        }

        public IEnumerable<T> GetItems<T>()
        {
            foreach (GameItem item in this)
                if (item is T needs) yield return needs;
        }

        public void ListChangedListening(Changed changed)
        {
            if (!DelegateTool.ExistInside(ListChanged, changed)) ListChanged += changed;
        }

        public void ListChangedRecall(Changed changed) => ListChanged -= changed;
    }

    //public class ItemsList : List<GameItem>, ICloneable
    //{
    //    public void AddGroup(GameItem newItem)
    //    {
    //        int count = newItem.Count;
    //        for (int i = 0; i < Count; i++)
    //        {
    //            newItem.Count = this[i].Count;
    //            if (this[i].Compare(newItem))
    //            {
    //                this[i].Count += count;
    //                return;
    //            }
    //        }
    //        newItem.Count = count;
    //        Add(newItem);
    //    }

    //    public void Shrink()
    //    {
    //        List<GameItem> items = new();
    //        for (int i = 0; i < Count; i++)
    //        {
    //            if (items.Count == 0 || items[Count - 1].GetType() != this[i].GetType())
    //            {
    //                var invokeGetItems = GetType().GetMethod("GetItems").MakeGenericMethod(this[i].GetType());
    //                items.AddRange(invokeGetItems.Invoke(this, new object[] { }) as GameItem[]);
    //                i = items.Count;
    //            }
    //        }
    //        Clear();
    //        AddRange(items);
    //    }

    //    public T[] GetItems<T>()
    //    {
    //        List<T> result = new();
    //        foreach (GameItem item in this)
    //            if (item is T necessaryItem) result.Add(necessaryItem);

    //        for (int i = 1; i < result.Count; i++)
    //        {
    //            GameItem key = result[i] as GameItem;
    //            int j = i - 1;
    //            while (j >= 0)
    //            {
    //                GameItem ignoreCount = (result[j] as GameItem).Clone() as GameItem;
    //                ignoreCount.Count = key.Count;
    //                if (ignoreCount.Compare(key))
    //                {
    //                    GameItem addCount = result[j] as GameItem;
    //                    addCount.Count += key.Count;
    //                    result.RemoveAt(i);
    //                    break;
    //                }
    //                j--;
    //            }
    //        }
    //        return result.ToArray();
    //    }

    //    public static bool operator !=(ItemsList a, ItemsList b) => !(a == b);
    //    public static bool operator ==(ItemsList a, ItemsList b)
    //    {
    //        bool nullA = false;
    //        bool nullB = false;
    //        try { _ = a.Count; } catch (NullReferenceException) { nullA = true; }
    //        try { _ = b.Count; } catch (NullReferenceException) { nullB = true; }

    //        if (nullA && nullB) return true;
    //        else if (nullB || nullB) return false;

    //        if (a.Count != b.Count) return false;
    //        else
    //        {
    //            for (int i = 0; i < b.Count; i++)
    //                if (!a[i].Compare(b[i])) return false;
    //            return true;
    //        }
    //    }

    //    public override bool Equals(object obj) => base.Equals(obj);
    //    public override int GetHashCode() => base.GetHashCode();
    //    public object Clone()
    //    {
    //        ItemsList clone = new();
    //        foreach (GameItem item in this)
    //            clone.Add(item.Clone() as GameItem);
    //        return clone;
    //    }
    //}


    public delegate void TreasureHunt();
    public delegate void AnswerTheQuestions(params Map[] questions);
    public delegate void CountingTime(object obj);

    public class GameRandom : System.Random
    {
        public int Next(ValuePair<int> pair)
            => new System.Random().Next(pair.MinValue, pair.MaxValue);

        public int Next(KeyValuePair<int, int> pair)
            => new System.Random().Next(pair.Key, pair.Value);

        public bool Probability(double ratio)
        {
            int[] fraction = ConvertToFraction(ratio);
            int i = Next(fraction[1]);
            if (i < fraction[0]) return true;
            else return false;
        }

        public int[] ConvertToFraction(double number)
        {
            const double epsilon = 1e-10;
            int maxDenominator = 100000;

            for (int denominator = 1; denominator <= maxDenominator; denominator++)
            {
                int numerator = (int)(number * denominator);
                double difference = Math.Abs(number - (double)numerator / denominator);

                if (difference < epsilon) return new int[] { numerator, denominator };
            }

            return new int[] { (int)Math.Round(number), 1 };
        }

        public T ChooseFromList<T>(params T[] array) => array[Next(array.Length)];

        public object Probability(params KeyValuePair<object, double>[] weightedObjects)
        {
            double totalWeight = 0;
            foreach (KeyValuePair<object, double> weightedObject in weightedObjects)
                totalWeight += weightedObject.Value;

            double randomWeight = NextDouble() * totalWeight;

            foreach (KeyValuePair<object, double> weightedObject in weightedObjects)
            {
                if (randomWeight < weightedObject.Value) return weightedObject.Key;
                
                randomWeight -= weightedObject.Value;
            }
            return null;
        }

        public void Shuffle<T>(T[] array)
        {
            int n = array.Length;

            while (n > 1)
            {
                n--;
                int k = Next(n + 1);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }
    }

    public struct GameConst
    {
        #region Block
        public const double HaveTimeToRecover = 0.6; // Truong hop random ra block can co thoi gian hoi phuc.

        public const double IsTrap = 0.2; // Truong hop block co bom.
        public const double Golds = 0.1; // Truong hop block chi co vang.
        public const double Items = 0.1; // Truong hop block co item khac.
        public const double Questions = 0.2; // Truong hop block co cau hoi.

        public static readonly ValuePair<int> RecoverInterval = new(240, 361); // Khoang thoii gian hoi phuc cua block.
        public static readonly ValuePair<int> InitInterval = new(180, 301); // Khoang thoi gian de khoi tao lai block.
        public static readonly ValuePair<int> Funds = new(10, 101); // Gioi han vang chua trong block.
        public static readonly int[] Explosion = new int[] { 20, 30, 40 }; // Cac muc vu no cua bom.
        #endregion
    }

    public struct ValuePair<T>
    {
        public ValuePair(T minValue, T maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public T MinValue;
        public T MaxValue;
    }
}
