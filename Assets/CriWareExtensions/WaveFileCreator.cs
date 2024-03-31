using System;
using System.IO;
using System.Text;

public class WaveFileCreator 
{
    private int numChannels = 2;
    private int samplingRate = 44100;
    private int numbites = 16;

    private BinaryWriter binaryWriter;

    public string FileFullPath {get;}
    
    public WaveFileCreator(string filePath, int numChannels, int samplingRate, int numbites) {
        this.numChannels = numChannels;
        this.samplingRate = samplingRate;
        this.numbites = numbites;

        FileFullPath = filePath;
        
        if (Path.HasExtension(FileFullPath) == false)
        {
            FileFullPath += ".wav";
        }

        var stream = new FileStream(FileFullPath, FileMode.Create);
        binaryWriter = new BinaryWriter(stream);

        //先頭ゼロ埋め//
        for (int n = 0; n < 44; n++) {
            binaryWriter.Write((byte)0);
        }
    }
    /* float(-1.0～+1.0)をshort(-32768～+32767)に変換*/
    static short ConvertFloatPcmToShortPcm(float value) 
    {
        long valueLong = (long)(value * short.MaxValue);
        return (short)(Math.Min(short.MaxValue, Math.Max(short.MinValue, valueLong)));
    }

    public void CapturePcm(float[] lChannel, float[] rChannel, int numSamples) {
        //LR交互に書き込み
        for (int n = 0; n < numSamples; n++) {
            binaryWriter.Write(BitConverter.GetBytes(ConvertFloatPcmToShortPcm(lChannel[n])));
            if (numChannels > 1) {
                binaryWriter.Write(BitConverter.GetBytes(ConvertFloatPcmToShortPcm(rChannel[n])));
            }
        }
    }

    public void StopAndWrite() {
        var writeBinaryWriter = binaryWriter;
        binaryWriter = null;

        //録音データの長さ//
        long length = writeBinaryWriter.BaseStream.Length;

        //0埋めしてたとこにヘッダ書き込み//
        writeBinaryWriter.Seek(0, SeekOrigin.Begin);

        //ヘッダー
        writeBinaryWriter.Write(Encoding.ASCII.GetBytes("RIFF"));
        writeBinaryWriter.Write((uint)(length - 8));//ファイルサイズ(riff と size を除くので-8）//
        writeBinaryWriter.Write(Encoding.ASCII.GetBytes("WAVE"));

        //fmtチャンク//
        writeBinaryWriter.Write(Encoding.ASCII.GetBytes("fmt "));
        writeBinaryWriter.Write((uint)16); //チャンクサイズ//
        writeBinaryWriter.Write((ushort)1);  //圧縮設定　１はPCMフォーマットです//
        writeBinaryWriter.Write((ushort)numChannels); //チャンネル数//
        writeBinaryWriter.Write((uint)samplingRate);  //1秒ごとのサンプル数、サンプリングレート//
        writeBinaryWriter.Write((uint)(samplingRate * numChannels * 2));//;//bytepersec 16 ビットステレオリニア PCM でサンプリング周波数
        writeBinaryWriter.Write((ushort)(numbites * numChannels / 8));   //ブロックアライン//
        writeBinaryWriter.Write((ushort)numbites); //bitswidth  1 サンプルあたりの使用するビット数//

        writeBinaryWriter.Write(Encoding.ASCII.GetBytes("data")); 　//こっからデータよ//
        writeBinaryWriter.Write((uint)(length - 44));//チャンクサイズ//
        writeBinaryWriter.Seek((int)length, SeekOrigin.Begin);

        //binaryWriter.Close();
        writeBinaryWriter.Flush();
        writeBinaryWriter.Close();
    }
}
