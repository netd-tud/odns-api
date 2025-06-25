namespace Utilities
{
    public enum FuzzyMatchingAlgo
    {
        LEVENSHTEIN=0
    }

    public class FuzzyMatching
    {
        private FuzzyMatchingAlgo _algo;

        public FuzzyMatching(FuzzyMatchingAlgo algo) 
        {
            _algo = algo;
        }

        public int FuzzyMatch(string s1, string s2, FuzzyMatchingAlgo? algo = null)
        {
            int similarity = -1;
            FuzzyMatchingAlgo algoToUse = _algo;
            try
            {
                if (algo != null) 
                {
                    algoToUse = (FuzzyMatchingAlgo)algo;
                }

                switch (algoToUse) 
                {
                    case FuzzyMatchingAlgo.LEVENSHTEIN:
                        similarity = LevenshteinDistance(s1, s2);
                    break;
                    default:
                        similarity = LevenshteinDistance(s1, s2);
                    break;
                }
            }
            catch (Exception e) 
            {

            }


            return similarity;
        }

        /// <summary>
        /// Calculates the Levenshtein distance between two strings.
        /// A lower number indicates a higher similarity.
        /// </summary>
        /// <returns>The number of edits required to transform s1 into s2.</returns>
        private int LevenshteinDistance(string s1, string s2)
        {
            s1 = s1.ToLowerInvariant();
            s2 = s2.ToLowerInvariant();
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                d[0, j] = j;

            for (int j = 1; j <= s2.Length; j++)
            {
                for (int i = 1; i <= s1.Length; i++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        d[i - 1, j] + 1,                         // Deletion
                        Math.Min(d[i, j - 1] + 1,               // Insertion
                                 d[i - 1, j - 1] + cost));      // Substitution
                }
            }
            return d[s1.Length, s2.Length];
        }
    }

}

