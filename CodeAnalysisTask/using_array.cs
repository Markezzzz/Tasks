public static void AddAndSort(ref KeyValuePair<int, string>[] array, KeyValuePair<int, string> keyValuePair)
{
    AddElement(ref array, keyValuePair);
    Sort(ref array);
}

private static void Sort(ref KeyValuePair<int, string>[] array)
{
    var count = 0;
    while (count != array.Length - 1)
    {
        count = 0;
        for (var j = array.Length - 1; j > 0; j--)
        {
            if (array[j - 1].Key <= array[j].Key)
            {
                count++;
                continue;
            }

            Swap(ref array, j, j - 1);
        }
    }
}

private static void Swap(ref KeyValuePair<int, string>[] array, int indexA, int indexB)
{
    var temp = array[indexA];
    array[indexA] = array[indexB];
    array[indexB] = temp;
}

private static void AddElement(ref KeyValuePair<int, string>[] array, KeyValuePair<int, string> keyValuePair)
{
    Array.Resize(ref array, array.Length + 1);
    array[array.Length - 1] = keyValuePair;
}
