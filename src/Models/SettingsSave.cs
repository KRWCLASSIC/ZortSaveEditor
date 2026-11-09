using System.Text.Json.Serialization;

namespace Zse.Models;

public sealed class SettingsSave
{
    [JsonPropertyName("screenWidth")]
    public int ScreenWidth { get; set; } = 1920;

    [JsonPropertyName("screenHeight")]
    public int ScreenHeight { get; set; } = 1080;

    [JsonPropertyName("displayIndex")]
    public int DisplayIndex { get; set; } = 0;

    [JsonPropertyName("fullscreenIndex")]
    public int FullscreenIndex { get; set; } = 0;

    [JsonPropertyName("graphicsIndex")]
    public int GraphicsIndex { get; set; } = 0;

    [JsonPropertyName("vsync")]
    public int Vsync { get; set; } = 0;

    [JsonPropertyName("targetFpsIndex")]
    public int TargetFpsIndex { get; set; } = 0;

    [JsonPropertyName("antiAliasing")]
    public bool AntiAliasing { get; set; } = true;

    [JsonPropertyName("sensitivityMouse")]
    public float SensitivityMouse { get; set; } = 3.0f;

    [JsonPropertyName("sensitivityController")]
    public float SensitivityController { get; set; } = 10.0f;

    [JsonPropertyName("enableController")]
    public bool EnableController { get; set; } = false;

    [JsonPropertyName("lensDistortion")]
    public float LensDistortion { get; set; } = 0.3f;

    [JsonPropertyName("motionBlur")]
    public bool MotionBlur { get; set; } = true;

    [JsonPropertyName("headBob")]
    public bool HeadBob { get; set; } = true;

    [JsonPropertyName("cameraLean")]
    public bool CameraLean { get; set; } = true;

    [JsonPropertyName("cameraEffects")]
    public bool CameraEffects { get; set; } = true;

    [JsonPropertyName("invertY")]
    public bool InvertY { get; set; } = false;

    [JsonPropertyName("invertScroll")]
    public bool InvertScroll { get; set; } = false;

    [JsonPropertyName("arachnophobiaMode")]
    public bool ArachnophobiaMode { get; set; } = false;

    [JsonPropertyName("masterVolume")]
    public float MasterVolume { get; set; } = 0.8f;

    [JsonPropertyName("voicechatOutputVolume")]
    public float VoicechatOutputVolume { get; set; } = 1.5f;

    [JsonPropertyName("voicechatInputVolume")]
    public float VoicechatInputVolume { get; set; } = 1.65f;

    [JsonPropertyName("subtitles")]
    public bool Subtitles { get; set; } = true;

    [JsonPropertyName("version")]
    public int Version { get; set; } = 20;
}

