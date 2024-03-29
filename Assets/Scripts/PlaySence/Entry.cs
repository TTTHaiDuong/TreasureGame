using System;
using System.Collections.Generic;
using System.Linq;
using GameItems;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace TreasureGame
{
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

                if (property1.GetValue(obj1) == property2.GetValue(obj2)) return false;
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

                if (field1.GetValue(obj1) == field2.GetValue(obj2)) return false;
            }
            return true;
        }
    }

    public class ItemList : List<GameItem>
    {
        public ItemList() : base()
        {
            ListChanged += RemoveEmptyItem;
        }

        public delegate void Changed();
        private event Changed ListChanged;

        private bool IsRangeChangedProcess;

        public new void Add(GameItem newItem)
        {
            foreach (GameItem item in this)
            {
                if (ObjectComparable.Compare(newItem, item, "Count"))
                {
                    item.Count += newItem.Count;
                    newItem = item;
                    return;
                }
            }
            base.Add(newItem);

            if (!IsRangeChangedProcess) ListChanged?.Invoke();
        }

        public new void AddRange(IEnumerable<GameItem> newItems)
        {
            if (newItems == null) return;
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

        private void RemoveEmptyItem()
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Count == 0) base.Remove(this[i]);
        }

        public void ListChangedListening(Changed changed)
        {
            if (!DelegateTool.ExistInside(ListChanged, changed)) ListChanged += changed;
        }

        public void ListChangedRecall(Changed changed) => ListChanged -= changed;
    }

    public delegate void TreasureHunt();

    public class GameRandom : System.Random
    {
        public int Next(ValuePair<int> pair)
            => new System.Random().Next(pair.MinValue, pair.MaxValue);

        public int Next(KeyValuePair<int, int> pair)
            => new System.Random().Next(pair.Key, pair.Value);

        public bool Probability(double ratio)
        {
            int[] fraction = ToFraction(ratio);
            int i = Next(fraction[1]);
            if (i < fraction[0]) return true;
            else return false;
        }

        public int[] ToFraction(double number)
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

        /// <summary>
        /// Chọn một phần tử bất kỳ trong một mảng.
        /// </summary>
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

        /// <summary>
        /// Xáo trộn các phần tử trong một mảng.
        /// </summary>
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

    /// <summary>
    /// Làm việc trực tiếp, truy xuất Database.
    /// </summary>
    public class DBProvider
    {
        public delegate void FailedConnectToServer(object obj);
        public static FailedConnectToServer FailedConnect;

        public static int ConnectionTime = 10;

        public static string ConnectionString = @"Data source=DUONG\HAIDUONG;Initial Catalog=TREASUREGAME;Integrated Security = True";

        #region Account
        public const string AccountTableName = "ACCOUNT";
        public const string AccountEmailColumnName = "EMAIL";
        public const string AccountPasswordColumnName = "PASSWORD";
        #endregion

        #region Student
        public const string StudentTableName = "PLAYER";
        public const string StudentIdColumn = "STUDENTID";
        public const string StudentNameColumn = "NAME";
        public const string StudentScoreColumn = "SCORE";
        public const string StudentAttendanceColumn = "ATTENDANCE";
        #endregion

        #region Questions
        public const string QuestionsTableName = "QUESTION";
        public const string QuestionColumn = "QUESTION";
        #endregion

        public static DataTable ExecuteQuery(string query)
        {
            DataTable table = new();
            using (SqlConnection connection = new(ConnectionString))
            {
                using (SqlCommand command = new(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new(command);
                        adapter.Fill(table);
                    }
                    catch (SqlException ex)
                    {
                        FailedConnect?.Invoke(ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        FailedConnect?.Invoke(ex);
                    }
                    catch (ArgumentException ex)
                    {
                        FailedConnect?.Invoke(ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return table;
        }

        public static DataTable ExecuteQuery(string query, string parameter, string value)
        {
            return ExecuteQuery(query, new string[] { parameter }, new string[] { value });
        }

        public static DataTable ExecuteQuery(string query, string[] parameters, string[] values)
        {
            DataTable table = new();
            using (SqlConnection connection = new(ConnectionString))
            {
                using (SqlCommand command = new(query, connection))
                {
                    try
                    {
                        if (parameters != null)
                        for (int i = 0; i < parameters.Length; i++)
                            command.Parameters.AddWithValue(parameters[i], values[i]);

                        connection.Open();
                        SqlDataAdapter adapter = new(command);
                        adapter.Fill(table);
                    }
                    catch (SqlException ex)
                    {
                        FailedConnect?.Invoke(ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        FailedConnect?.Invoke(ex);
                    }
                    catch (ArgumentException ex)
                    {
                        FailedConnect?.Invoke(ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return table;
        }
    }

    public struct GameConst
    {
        #region Block
        public const double HaveTimeToRecover = 0.6; // Truong hop random ra block can co thoi gian hoi phuc.

        public const double IsTrap = 0.15; // Truong hop block co bom.
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
