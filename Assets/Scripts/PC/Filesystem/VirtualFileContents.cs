using UnityEngine;

public abstract class VirtualFileContents
{ }

public class TextContents : VirtualFileContents
{
    public string Value { get; set; }
}

public class ImageContents : VirtualFileContents
{
    public Texture Value { get; set; }
}