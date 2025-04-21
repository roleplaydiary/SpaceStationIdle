using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Resources : IEnumerable<KeyValuePair<string, float>>
{
    public float Phoron;
    public float Metal;
    public float Glass;
    public float Plastic;
    public float Gold;
    public float Silver;
    public float Uranium;

    public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
    {
        yield return new KeyValuePair<string, float>(nameof(Phoron), Phoron);
        yield return new KeyValuePair<string, float>(nameof(Metal), Metal);
        yield return new KeyValuePair<string, float>(nameof(Glass), Glass);
        yield return new KeyValuePair<string, float>(nameof(Plastic), Plastic);
        yield return new KeyValuePair<string, float>(nameof(Gold), Gold);
        yield return new KeyValuePair<string, float>(nameof(Silver), Silver);
        yield return new KeyValuePair<string, float>(nameof(Uranium), Uranium);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public enum ResourceType
{
    Phoron,
    Metal,
    Glass,
    Plastic,
    Gold,
    Silver,
    Uranium
}