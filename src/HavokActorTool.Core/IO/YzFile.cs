using System.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using CsYaz0;
using CsYaz0.Marshalling;

namespace HavokActorTool.Core.IO;

public static class YzFile
{
    public static SpanOwner<byte> OpenAndDecompress(string file)
    {
        using FileStream fs = File.OpenRead(file);
        int size = (int)fs.Length;
        using SpanOwner<byte> compressedBuffer = SpanOwner<byte>.Allocate(size);
        int read = fs.Read(compressedBuffer.Span);
        Debug.Assert(read == size);

        size = Yaz0.GetDecompressedSize(compressedBuffer.Span);
        SpanOwner<byte> decompressedBuffer = SpanOwner<byte>.Allocate(size); 
        Yaz0.Decompress(compressedBuffer.Span, decompressedBuffer.Span);
        return decompressedBuffer;
    }
    
    public static void WriteAndCompress(FileInfo outputFile, Action<Stream> write)
    {
        using MemoryStream ms = new();
        write(ms);
        using DataMarshal compressed = Yaz0.Compress(ms.ToArray());
        
        outputFile.Directory?.Create();
        using FileStream outputStream = outputFile.Create();
        outputStream.Write(compressed.AsSpan());
    }
}