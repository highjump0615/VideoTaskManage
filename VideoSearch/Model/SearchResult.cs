using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using VideoSearch.SearchListView;

namespace VideoSearch.Model
{
    public class SearchResult : ObservableCollection<SearchResultItem>
    {
        public static SearchResult _searchResult = null;

        public static SearchResult Result
        {
            get
            {
                if (_searchResult == null)
                    _searchResult = new SearchResult();

                return _searchResult;
            }
        }

        public static void ClearResult()
        {
            Result.Clear();
        }

        public static void AddResult(FormattedText result)
        {
            Result.Add(new SearchResultItem(result));
        }

        public static void RemoveResult(FormattedText result)
        {
            foreach(SearchResultItem item in Result)
            {
                if (item.ItemText == result)
                {
                    Result.Remove(item);
                    break;
                }
            }
        }

        public static void UpdateResult(FormattedText oldResult, FormattedText newResult)
        {
            foreach (SearchResultItem item in Result)
            {
                if (item.ItemText == oldResult)
                {
                    Result[Result.IndexOf(item)] = new SearchResultItem(newResult);
                    break;
                }
            }
        }

        public static int GetCount()
        {
            return Result.Count;
        }
    }


}
