using System;
using System.Collections.Generic;

public static class EnumerableExtensions
{
    /// <summary>
    /// Gibt das Element aus der Sequenz zurück, bei dem der Wert aus dem Selector minimal ist.
    /// Verwendet den Default-Vergleicher für den Key.
    /// </summary>
    /// <typeparam name="TSource">Typ der Elemente in der Sequenz.</typeparam>
    /// <typeparam name="TKey">Typ des ausgewerteten Werts, der vergleichbar sein muss.</typeparam>
    /// <param name="source">Die Quellsequenz.</param>
    /// <param name="selector">Funktion, die aus einem Element den zu vergleichenden Key liefert.</param>
    /// <returns>Das Element, dessen Key minimal ist.</returns>
    /// <exception cref="ArgumentNullException">Falls source oder selector null sind.</exception>
    /// <exception cref="InvalidOperationException">Falls die Sequenz leer ist.</exception>
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
    {
        return MinBy(source, selector, Comparer<TKey>.Default);
    }

    /// <summary>
    /// Gibt das Element aus der Sequenz zurück, bei dem der Wert aus dem Selector minimal ist.
    /// Ermöglicht die Angabe eines benutzerdefinierten Comparers.
    /// </summary>
    /// <typeparam name="TSource">Typ der Elemente in der Sequenz.</typeparam>
    /// <typeparam name="TKey">Typ des ausgewerteten Werts, der vergleichbar sein muss.</typeparam>
    /// <param name="source">Die Quellsequenz.</param>
    /// <param name="selector">Funktion, die aus einem Element den zu vergleichenden Key liefert.</param>
    /// <param name="comparer">Der zu verwendende Comparer für den Key.</param>
    /// <returns>Das Element, dessen Key minimal ist.</returns>
    /// <exception cref="ArgumentNullException">Falls source, selector oder comparer null sind.</exception>
    /// <exception cref="InvalidOperationException">Falls die Sequenz leer ist.</exception>
    public static TSource MinBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> selector,
        IComparer<TKey> comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        using (var enumerator = source.GetEnumerator())
        {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            TSource minElement = enumerator.Current;
            TKey minKey = selector(minElement);

            while (enumerator.MoveNext())
            {
                TSource current = enumerator.Current;
                TKey currentKey = selector(current);
                if (comparer.Compare(currentKey, minKey) < 0)
                {
                    minKey = currentKey;
                    minElement = current;
                }
            }
            return minElement;
        }
    }
    /// <summary>
    /// Gibt das Element aus der Sequenz zurück, bei dem der Wert aus dem Selector maximal ist.
    /// Verwendet den Default-Vergleicher für den Key.
    /// </summary>
    /// <typeparam name="TSource">Typ der Elemente in der Sequenz.</typeparam>
    /// <typeparam name="TKey">Typ des ausgewerteten Werts, der vergleichbar sein muss.</typeparam>
    /// <param name="source">Die Quellsequenz.</param>
    /// <param name="selector">Funktion, die aus einem Element den zu vergleichenden Key liefert.</param>
    /// <returns>Das Element, dessen Key maximal ist.</returns>
    /// <exception cref="ArgumentNullException">Falls source oder selector null sind.</exception>
    /// <exception cref="InvalidOperationException">Falls die Sequenz leer ist.</exception>
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
    {
        return MaxBy(source, selector, Comparer<TKey>.Default);
    }

    /// <summary>
    /// Gibt das Element aus der Sequenz zurück, bei dem der Wert aus dem Selector maximal ist.
    /// Ermöglicht die Angabe eines benutzerdefinierten Comparers.
    /// </summary>
    /// <typeparam name="TSource">Typ der Elemente in der Sequenz.</typeparam>
    /// <typeparam name="TKey">Typ des ausgewerteten Werts, der vergleichbar sein muss.</typeparam>
    /// <param name="source">Die Quellsequenz.</param>
    /// <param name="selector">Funktion, die aus einem Element den zu vergleichenden Key liefert.</param>
    /// <param name="comparer">Der zu verwendende Comparer für den Key.</param>
    /// <returns>Das Element, dessen Key maximal ist.</returns>
    /// <exception cref="ArgumentNullException">Falls source, selector oder comparer null sind.</exception>
    /// <exception cref="InvalidOperationException">Falls die Sequenz leer ist.</exception>
    public static TSource MaxBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> selector,
        IComparer<TKey> comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        using (var enumerator = source.GetEnumerator())
        {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            TSource maxElement = enumerator.Current;
            TKey maxKey = selector(maxElement);

            while (enumerator.MoveNext())
            {
                TSource current = enumerator.Current;
                TKey currentKey = selector(current);
                if (comparer.Compare(currentKey, maxKey) > 0)
                {
                    maxKey = currentKey;
                    maxElement = current;
                }
            }
            return maxElement;
        }
    }

    /// <summary>
    /// Ermittelt den Index des Elements in der Sequenz, bei dem der Wert aus dem Selector minimal ist.
    /// Verwendet den Default-Vergleicher für den Key.
    /// </summary>
    /// <typeparam name="TSource">Typ der Elemente in der Sequenz.</typeparam>
    /// <typeparam name="TKey">Typ des ausgewerteten Werts, der vergleichbar sein muss.</typeparam>
    /// <param name="source">Die Quellsequenz.</param>
    /// <param name="selector">Funktion, die aus einem Element den zu vergleichenden Key liefert.</param>
    /// <returns>Der Index des Elements mit dem minimalen Key.</returns>
    /// <exception cref="ArgumentNullException">Falls source oder selector null sind.</exception>
    /// <exception cref="InvalidOperationException">Falls die Sequenz leer ist.</exception>
    public static int IndexOfMinBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> selector)
    {
        return IndexOfMinBy(source, selector, Comparer<TKey>.Default);
    }

    /// <summary>
    /// Ermittelt den Index des Elements in der Sequenz, bei dem der Wert aus dem Selector minimal ist.
    /// Ermöglicht die Angabe eines benutzerdefinierten Comparers.
    /// </summary>
    /// <typeparam name="TSource">Typ der Elemente in der Sequenz.</typeparam>
    /// <typeparam name="TKey">Typ des ausgewerteten Werts, der vergleichbar sein muss.</typeparam>
    /// <param name="source">Die Quellsequenz.</param>
    /// <param name="selector">Funktion, die aus einem Element den zu vergleichenden Key liefert.</param>
    /// <param name="comparer">Der zu verwendende Comparer für den Key.</param>
    /// <returns>Der Index des Elements mit dem minimalen Key.</returns>
    /// <exception cref="ArgumentNullException">Falls source, selector oder comparer null sind.</exception>
    /// <exception cref="InvalidOperationException">Falls die Sequenz leer ist.</exception>
    public static int IndexOfMinBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> selector,
        IComparer<TKey> comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        int index = 0;
        int minIndex = 0;
        bool foundAny = false;
        TKey minKey = default;

        foreach (TSource item in source)
        {
            if (!foundAny)
            {
                // Erstes Element: Initialisierung der Minimalwerte.
                minKey = selector(item);
                minIndex = index;
                foundAny = true;
            }
            else
            {
                TKey currentKey = selector(item);
                if (comparer.Compare(currentKey, minKey) < 0)
                {
                    minKey = currentKey;
                    minIndex = index;
                }
            }
            index++;
        }

        if (!foundAny)
            throw new InvalidOperationException("Die Sequenz enthält keine Elemente.");

        return minIndex;
    }
}
