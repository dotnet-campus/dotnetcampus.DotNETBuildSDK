using System.Collections.Generic;
using System.Linq;

namespace dotnetCampus.MatrixRun
{
    /// <summary>
    /// 帮助计算笛卡尔集。
    /// </summary>
    public static class CartesianProduct
    {
        /// <summary>
        /// 返回指定集合的笛卡尔集（即所有项的全部组合）。
        /// 例如原集合是：{ A, B, C }, { 1, 2 }, { x, y, z }
        /// 那么返回的新集合是：
        /// { A, 1, x }, { A, 1, y }, { A, 1, z },
        /// { A, 2, x }, { A, 2, y }, { A, 2, z },
        /// { B, 1, x }, { B, 1, y }, { B, 1, z },
        /// { B, 2, x }, { B, 2, y }, { B, 2, z },
        /// { C, 1, x }, { C, 1, y }, { C, 1, z },
        /// { C, 2, x }, { C, 2, y }, { C, 2, z },
        /// </summary>
        /// <typeparam name="T">任意集合类型。</typeparam>
        /// <param name="lists">要计算笛卡尔集的原集合。</param>
        /// <returns>笛卡尔集，即参数集合的所有可能组合。</returns>
        public static IEnumerable<IReadOnlyList<T>> Enumerate<T>(IEnumerable<IReadOnlyList<T>> lists)
        {
            // cartesianCount: 笛卡尔集中的集合总数
            ulong cartesianCount = lists.Select(x => (ulong)x.Count).Aggregate(1ul, (a, b) => a * b);

            // globalIndex: 当前正在计算的组合在整个笛卡尔集所有组合中的序号
            for (var globalIndex = 0ul; globalIndex < cartesianCount; globalIndex++)
            {
                // output: 当前序号下的一个组合
                var output = new List<T>();

                // otherCount: 除去当前正在计算的组合外剩余组合的个数（用于计算每一子项的序号）
                ulong otherCount = cartesianCount;
                // selfIndex: 当前正在计算的这种组合的序号
                // selfCount: 当前正在计算的这种组合的当前位置所取的原集合的个数
                ulong selfIndex, selfCount;
                foreach (var list in lists)
                {
                    selfCount = (ulong)list.Count;
                    otherCount /= selfCount;
                    selfIndex = (globalIndex / otherCount) % selfCount;
                    output.Add(list[(int)selfIndex]);
                }

                yield return output;
            }
        }

        /// <summary>
        /// 返回带有 Key 标记的指定集合的笛卡尔集。
        /// 例如原集合是：α={ A, B, C }, β={ 1, 2 }, γ={ x, y, z }
        /// 那么返回的新集合是：
        /// { α=A, β=1, γ=x }, { α=A, β=1, γ=y }, { α=A, β=1, γ=z },
        /// { α=A, β=2, γ=x }, { α=A, β=2, γ=y }, { α=A, β=2, γ=z },
        /// { α=B, β=1, γ=x }, { α=B, β=1, γ=y }, { α=B, β=1, γ=z },
        /// { α=B, β=2, γ=x }, { α=B, β=2, γ=y }, { α=B, β=2, γ=z },
        /// { α=C, β=1, γ=x }, { α=C, β=1, γ=y }, { α=C, β=1, γ=z },
        /// { α=C, β=2, γ=x }, { α=C, β=2, γ=y }, { α=C, β=2, γ=z },
        /// </summary>
        /// <typeparam name="TKey">Key 标记的类型。</typeparam>
        /// <typeparam name="TValue">项类型。</typeparam>
        /// <param name="dictionary">原集合。</param>
        /// <returns>带有 Key 标记的笛卡尔集，即参数集合的所有可能组合。</returns>
        public static IEnumerable<IReadOnlyDictionary<TKey, TValue>> Enumerate<TKey, TValue>(
            IReadOnlyDictionary<TKey, IReadOnlyList<TValue>> dictionary)
            where TKey : notnull
        {
            var keys = dictionary.Keys.ToList();
            var values = dictionary.Values.ToList();
            foreach (var valueCombination in Enumerate(values))
            {
                yield return valueCombination
                    .Select((x, i) => new KeyValuePair<TKey, TValue>(keys[i], x))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
