using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Anagramimation
{

public class AnagramDictionary
{
    public static readonly Lazy<AnagramDictionary> Default = new(
        () => new AnagramDictionary(Words.AllWords));


        public static readonly Lazy<AnagramDictionary> Animals = new(
            () => new AnagramDictionary(Words.Animals));

        public AnagramDictionary(string s)
    {
        var words = s.Split(
            new []{'\r','\n'},
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );

        Dictionary = words.Select(word => (key: AnagramKey.FromString(word), word))
            .GroupBy(x => x.key, x => x.word)
            .ToDictionary(x => x.Key, x => x.ToImmutableList());
    }

    public readonly IReadOnlyDictionary<AnagramKey, ImmutableList<string>> Dictionary;


    public IEnumerable<ImmutableList<string>> GetAnagrams(string text, int maxWords)
    {
        var key = AnagramKey.FromString(text);

        return GetAnagrams(key, maxWords);
    }

    public IEnumerable<ImmutableList<string>> GetAnagrams(AnagramKey key, int maxWords)
    {

        if (key.Length == 0)
        {
            yield return ImmutableList<string>.Empty;

            yield break;
        }

        if(maxWords <= 0)
            yield break;

        if (Dictionary.TryGetValue(key, out var list))
            foreach (var w in list)
                yield return ImmutableList<string>.Empty.Add(w);


        if(maxWords <= 1)
            yield break;

        for (var i = key.Length / 2; i > 0; i--)
        {
            foreach (var (left, right) in key.GetPairs(i, 'a'))
            {
                if (Dictionary.TryGetValue(left, out var leftWordList) && leftWordList.Any())
                {
                    foreach (var anagram in GetAnagrams(right, maxWords - 1))
                    {
                        foreach (var l in leftWordList)
                        {
                            yield return anagram.Add(l);
                        }
                    }
                }
            }
        }
    }
}

public sealed record AnagramKey(ImmutableSortedDictionary<char, int> Dict)
{
    public static AnagramKey FromString(string s)
    {
        var dict =
            s.ToLowerInvariant()
                .Trim()
                .Where(char.IsLetter)
                .GroupBy(x => x)
                .ToImmutableSortedDictionary(x => x.Key, x => x.Count());

        return new AnagramKey(dict);
    }

    public static readonly AnagramKey Empty = new(ImmutableSortedDictionary<char, int>.Empty);

    public static readonly IReadOnlyDictionary<char, AnagramKey> FromChar =
        Enumerable.Range('a', 26)
            .ToDictionary(
                x => (char)x,
                x =>
                    new AnagramKey(ImmutableSortedDictionary<char, int>.Empty.Add((char)x, 1))
            );

    public AnagramKey Combine(AnagramKey other)
    {
        if (Dict.IsEmpty)
            return other;

        if (other.Dict.IsEmpty)
            return this;

        var newItems = other.Dict.Select(
            kvp =>
                Dict.TryGetValue(kvp.Key, out var v2)
                    ? new KeyValuePair<char, int>(kvp.Key, kvp.Value + v2)
                    : kvp
        );

        var newDict = Dict.SetItems(newItems);

        return new AnagramKey(newDict);
    }

    private string? _key;

    public string Key => _key
        ??= new string(Dict.SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToArray());

    public IEnumerable<(AnagramKey left, AnagramKey right)> GetPairs(
        int rightLength,
        char firstChar)
    {
        AnagramKey GetNewLeft(char key, int oldValue)
        {
            return oldValue <= 1
                ? new AnagramKey(Dict.Remove(key))
                : new AnagramKey(Dict.SetItem(key, oldValue - 1));
        }

        if (rightLength <= 0)
            yield return (this, Empty);
        else if (rightLength == 1)
        {
            foreach (var (key, value) in Dict.Where(x => x.Key >= firstChar))
            {
                AnagramKey newLeft = GetNewLeft(key, value);
                var        right   = FromChar[key];

                yield return (newLeft, right);
            }
        }
        else if (rightLength > Length / 2)
        {
            foreach (var (left, right) in GetPairs(Length - rightLength, firstChar))
                yield return (right, left);
        }
        else
        {
            foreach (var (key, value) in Dict.Where(x => x.Key >= firstChar))
            {
                AnagramKey newLeft = GetNewLeft(key, value);
                var        right   = FromChar[key];

                foreach (var (finalLeft, finalRight) in
                    newLeft.GetPairs(rightLength - 1, key))
                    yield return (finalLeft, finalRight.Combine(right));
            }
        }
    }

    public int Length => Key.Length;

    /// <inheritdoc />
    public override string ToString() => Key;

    /// <inheritdoc />
    public override int GetHashCode() => Key.GetHashCode();

    bool IEquatable<AnagramKey?>.Equals(AnagramKey? other) =>
        other is not null && Key.Equals(other.Key);
}

}
