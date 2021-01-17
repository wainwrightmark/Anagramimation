using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Anagramimation
{

public class AnagramDictionary
{
    public static readonly Lazy<AnagramDictionary> Default = new(
        () => new AnagramDictionary(new Words.DictionaryHelper(2).AllWords.Value));


    public AnagramDictionary(IEnumerable<string> words)
    {
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

        for (var firstSubstringLength = key.Length / 2; firstSubstringLength > 0; firstSubstringLength--)
        {
            foreach (var substring in key.GetSubstrings(firstSubstringLength, 'a'))
            {
                if (!Dictionary.TryGetValue(substring, out var words))
                    continue;

                var remainder = key.TrySubtract(substring);

                if (remainder == null)
                    continue;

                foreach (var wordList in GetAnagrams(remainder, maxWords - 1))
                foreach (var word in words)
                    yield return wordList.Add(word);
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

    public AnagramKey Add(AnagramKey other)
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

    public AnagramKey? TrySubtract(AnagramKey other)
    {
        if (other.Dict.IsEmpty)
            return this;

        var newDict = Dict;

        foreach (var (key, amountToSubtract) in other.Dict)
        {
            if (newDict.TryGetValue(key, out var oldValue))
            {
                if(amountToSubtract > oldValue)return null;
                if (amountToSubtract == oldValue)
                    newDict = newDict.Remove(key);
                else
                    newDict = newDict.SetItem(key, oldValue - amountToSubtract);
            }
        }

        return new AnagramKey(newDict);
    }

        private string? _key;

    public string Key => _key
        ??= new string(Dict.SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToArray());


    public IEnumerable<AnagramKey> GetSubstrings(int length, char firstChar)
    {
        if(length > Length)
            yield break;

        if (length == Length)
        {
            yield return this;
            yield break;
        }

        foreach (var (key, oldValue) in Dict.Where(x=>x.Key >= firstChar))
        {
                var newKey = oldValue <= 1
                 ? new AnagramKey(Dict.Remove(key))
                 : new AnagramKey(Dict.SetItem(key, oldValue - 1));

                foreach (var ak in newKey.GetSubstrings(length, key))
                    yield return ak;
        }
    }



    /*
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
    }*/

    public int Length => Key.Length;

    /// <inheritdoc />
    public override string ToString() => Key;

    /// <inheritdoc />
    public override int GetHashCode() => Key.GetHashCode();

    bool IEquatable<AnagramKey?>.Equals(AnagramKey? other) =>
        other is not null && Key.Equals(other.Key);
}

}
