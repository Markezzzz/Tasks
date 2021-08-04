public IList<KeyValuePair<int, string>> AddAndSort(IList<KeyValuePair<int, string>> list, KeyValuePair<int, string> keyValuePair)
{
    list.Add(keyValuePair);
    return Sort(list);
}

private static IList<KeyValuePair<int, string>> Sort(IList<KeyValuePair<int, string>> list)
{
    var sortedList = list;
    var count = 0;
    while (count != list.Count - 1)
    {
        count = 0;
        for (var j = list.Count - 1; j > 0; j--)
        {
            if (list[j - 1].Key <= list[j].Key)
            {
                count++;
                continue;
            }

            sortedList = Swap(list, j, j - 1);
        }
    }

    return sortedList;
}

private static IList<KeyValuePair<int, string>> Swap(IList<KeyValuePair<int, string>> list, int indexA, int indexB)
{
    var temp = list[indexA];
    list[indexA] = list[indexB];
    list[indexB] = temp;
    return list;
}
