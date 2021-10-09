using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FeatureExtraction
{
    public partial class WordExtractor
    {

        // It's used as a key in a dictionary
        private struct WordKey
        {
            public string Term;
            public int DocumentIndex;

            public WordKey(string term, int documentIndex)
            {
                this.Term = term;
                this.DocumentIndex = documentIndex;
            }
        }
        

        private static Regex removeNonAlphanumericRegex = new Regex("[^A-Za-z0-9áéíóúÁÉÍÓÚñÑ -]");

        private static IEnumerable<string> Tokenize(string text)
        {
            text = removeNonAlphanumericRegex.Replace(text, "");

            return text.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);
        }

        public IDictionary<string, int> GetTermFrecuency(string text)
        {
            var bag = new Dictionary<string, int>();

            foreach (string word in Tokenize(text))
            {
                if (string.IsNullOrWhiteSpace(word)) continue;

                if (! bag.ContainsKey(word))
                {
                    bag[word] = 1;
                }
                else
                {
                    bag[word] += 1;
                }
            }

            return bag;
        }

        private IDictionary<WordKey, int> GetVocabulary(string text, out IDictionary<string, int>[] documents)
        {
            documents = text.ToLower()
                .Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(document => GetTermFrecuency(document))
                .ToArray();

            var vocabulary = new Dictionary<WordKey, int>();

            for (int documentIndex = 0; documentIndex < documents.Length; documentIndex++)
            {
                foreach ((string term, int occurencesCount) in documents[documentIndex])
                {
                    var word = new WordKey(term, documentIndex);

                    if (! vocabulary.ContainsKey(word))
                    {
                        vocabulary[word] = occurencesCount;
                    }
                    else
                    {
                        vocabulary[word] += occurencesCount;
                    }
                }
            }

            return vocabulary;
        }

        public IEnumerable<Feature> GetTermFrecuencyTimesInverseDocumentFrecuency(string text)
        {
            IDictionary<string, int>[] documents;

            IDictionary<WordKey, int> vocabulary = GetVocabulary(text, out documents);

            Dictionary<WordKey, double> results = new Dictionary<WordKey, double>(); // Key = word, value = tf*idf value

            foreach ((WordKey word, int numberOfDocumentsContainingThatWord) in vocabulary)
            {
                // Inverse document frecuency
                double idf = Math.Log((double)documents.Length / numberOfDocumentsContainingThatWord);

                // Term frecuncy
                double tf = documents[word.DocumentIndex].Count(kv => kv.Key == word.Term);
                results[word] = tf * idf;
            }


            return results.Select(kv => new Feature
            {
                Word = kv.Key.Term,
                DocumentIndex = kv.Key.DocumentIndex,
                Weight = kv.Value,
            });
        }

    }
}
