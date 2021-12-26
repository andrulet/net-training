using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Collections.Tasks {

    /// <summary>
    ///  Tree node item 
    /// </summary>
    /// <typeparam name="T">the type of tree node data</typeparam>
    public interface ITreeNode<T> {
        T Data { get; set; }                             // Custom data
        IEnumerable<ITreeNode<T>> Children { get; set; } // List of childrens
    }


    public class Task {

        /// <summary> Generate the Fibonacci sequence f(x) = f(x-1)+f(x-2) </summary>
        /// <param name="count">the size of a required sequence</param>
        /// <returns>
        ///   Returns the Fibonacci sequence of required count
        /// </returns>
        /// <exception cref="ArgumentException">count is less then 0</exception>
        /// <example>
        ///   0 => { }  
        ///   1 => { 1 }    
        ///   2 => { 1, 1 }
        ///   12 => { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144 }
        /// </example>
        public static IEnumerable<int> GetFibonacciSequence(int count) 
        {
            if (count < 0)
            {
                throw new ArgumentException("count");
            }

            //var result = new List<int>();
            //if (count == 0)
            //{
            //    return result;
            //}

            //if (count <= 1 || count > 1)
            //{
            //    result.Add(1);
            //}
            //if (count <= 2 || count > 2)
            //{
            //    result.Add(1);
            //}
            //int x = 1;
            //while (x < count - 1)
            //{
            //    result.Add(result[x] + result[x - 1]);
            //    x++;
            //}

            //return result;
            int previous = 0;
            int current = 1;
            int step = 0;

            while (step < count)
            {
                yield return current;

                int newCurrent = previous + current;
                previous = current;
                current = newCurrent;
                step++;
            }
        }

        /// <summary>
        ///    Parses the input string sequence into words
        /// </summary>
        /// <param name="reader">input string sequence</param>
        /// <returns>
        ///   The enumerable of all words from input string sequence. 
        /// </returns>
        /// <exception cref="ArgumentNullException">reader is null</exception>
        /// <example>
        ///  "TextReader is the abstract base class of StreamReader and StringReader, which ..." => 
        ///   {"TextReader","is","the","abstract","base","class","of","StreamReader","and","StringReader","which",...}
        /// </example>
        public static IEnumerable<string> Tokenize(TextReader reader) {
            char[] delimeters = new[] { ',', ' ', '.', '\t', '\n' };
            if(reader is null)
            {
                throw new ArgumentNullException();
            }

            var charList = new List<char>();
            int position;
            while((position = reader.Read()) != -1)
            {
                charList.Add((char)position);
            }

            return new String(charList.ToArray()).Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
        }



        /// <summary>
        ///   Traverses a tree using the depth-first strategy
        /// </summary>
        /// <typeparam name="T">tree node type</typeparam>
        /// <param name="root">the tree root</param>
        /// <returns>
        ///   Returns the sequence of all tree node data in depth-first order
        /// </returns>
        /// <example>
        ///    source tree (root = 1):
        ///    
        ///                      1
        ///                    / | \
        ///                   2  6  7
        ///                  / \     \
        ///                 3   4     8
        ///                     |
        ///                     5   
        ///                   
        ///    result = { 1, 2, 3, 4, 5, 6, 7, 8 } 
        /// </example>
        public static IEnumerable<T> DepthTraversalTree<T>(ITreeNode<T> root) {
            if (root == null)
            {
                throw new ArgumentNullException();
            }

            Stack<ITreeNode<T>> stack = new Stack<ITreeNode<T>>();
            List<ITreeNode<T>> list = new List<ITreeNode<T>>();
            stack.Push(root);

            while (stack.Count != 0)
            {
                var node = stack.Pop();                
                if(node.Children != null)
                {
                    list.AddRange(node.Children);
                    list.Reverse();
                    foreach (var x in list)
                    {
                        stack.Push(x);
                    }

                    list.Clear();
                }
                yield return node.Data;
            }

            //if (root.Children == null)       //recursion
            //{
            //    return new List<T> { root.Data };
            //}

            //var result = new List<T>();
            //if(root.Children != null)
            //{
            //    result.Add(root.Data);
            //    foreach (var child in root.Children)
            //    {
            //        result.AddRange(DepthTraversalTree(child));
            //    }
            //}
            //else
            //{
            //    result.Add(root.Data);
            //}

            //return result;
        }

        /// <summary>
        ///   Traverses a tree using the width-first strategy
        /// </summary>
        /// <typeparam name="T">tree node type</typeparam>
        /// <param name="root">the tree root</param>
        /// <returns>
        ///   Returns the sequence of all tree node data in width-first order
        /// </returns>
        /// <example>
        ///    source tree (root = 1):
        ///    
        ///                      1
        ///                    / | \
        ///                   2  3  4
        ///                  / \     \
        ///                 5   6     7
        ///                     |
        ///                     8   
        ///                   
        ///    result = { 1, 2, 3, 4, 5, 6, 7, 8 } 
        /// </example>
        public static IEnumerable<T> WidthTraversalTree<T>(ITreeNode<T> root) {
            if (root == null)
            {
                throw new ArgumentNullException();
            }

            Queue<ITreeNode<T>> treeNodes = new Queue<ITreeNode<T>>();
            treeNodes.Enqueue(root);

            while (treeNodes.Count != 0)
            {
                var node = treeNodes.Dequeue();                
                if (node.Children != null)
                {
                    foreach (var x in node.Children)
                    {
                        treeNodes.Enqueue(x);
                    }
                }
                yield return node.Data;
            }
        }



        /// <summary>
        ///   Generates all permutations of specified length from source array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source array</param>
        /// <param name="count">permutation length</param>
        /// <returns>
        ///    All permuations of specified length
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">count is less then 0 or greater then the source length</exception>
        /// <example>
        ///   source = { 1,2,3,4 }, count=1 => {{1},{2},{3},{4}}
        ///   source = { 1,2,3,4 }, count=2 => {{1,2},{1,3},{1,4},{2,3},{2,4},{3,4}}
        ///   source = { 1,2,3,4 }, count=3 => {{1,2,3},{1,2,4},{1,3,4},{2,3,4}}
        ///   source = { 1,2,3,4 }, count=4 => {{1,2,3,4}}
        ///   source = { 1,2,3,4 }, count=5 => ArgumentOutOfRangeException
        /// </example>
        public static IEnumerable<T[]> GenerateAllPermutations<T>(T[] source, int count) {
            if(count > source.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0)
            {
                yield break;
            }
            else if(count == 1)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    yield return new T[] { source[i] };
                }
            }
            else if (count == source.Length)
            {
                yield return source;
            }
            else
            {

                foreach (var x in source.Combinations(count))
                {
                    yield return x.ToArray();
                }                
            }
        }
    }



    public static class DictionaryExtentions {
        
        /// <summary>
        ///    Gets a value from the dictionary cache or build new value
        /// </summary>
        /// <typeparam name="TKey">TKey</typeparam>
        /// <typeparam name="TValue">TValue</typeparam>
        /// <param name="dictionary">source dictionary</param>
        /// <param name="key">key</param>
        /// <param name="builder">builder function to build new value if key does not exist</param>
        /// <returns>
        ///   Returns a value assosiated with the specified key from the dictionary cache. 
        ///   If key does not exist than builds a new value using specifyed builder, puts the result into the cache 
        ///   and returns the result.
        /// </returns>
        /// <example>
        ///   IDictionary<int, Person> cache = new SortedDictionary<int, Person>();
        ///   Person value = cache.GetOrBuildValue(10, ()=>LoadPersonById(10) );  // should return a loaded Person and put it into the cache
        ///   Person cached = cache.GetOrBuildValue(10, ()=>LoadPersonById(10) );  // should get a Person from the cache
        /// </example>
        public static TValue GetOrBuildValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> builder) {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, builder());
            }
            if (dictionary.TryGetValue(key, out var result))
            {
            }

            return result;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }
}
