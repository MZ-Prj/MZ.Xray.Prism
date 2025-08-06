using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using System;

namespace MZ.Resource.Managers
{
    /// <summary>
    /// Zeffect ShaderEffect 클래스
    /// </summary>
    public class ZeffShaderManager : ShaderEffect
    {

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ZeffShaderManager), 0);

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(float), typeof(ZeffShaderManager), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(float), typeof(ZeffShaderManager), new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ZeffShaderManager), new UIPropertyMetadata(Colors.Yellow, PixelShaderConstantCallback(2)));

        private static readonly PixelShader _pixelShader = new()
        {
            UriSource = new Uri("/MZ.Resource;component/Shaders/zeff.ps", UriKind.Relative)
        };

        public ZeffShaderManager()
        {
            this.PixelShader = _pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(MinProperty);
            UpdateShaderValue(MaxProperty);
            UpdateShaderValue(ColorProperty);
        }

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set
            {
                if (value == null)
                {
                    SetValue(InputProperty, new SolidColorBrush(Colors.Transparent));
                }
                else if (value is Brush brush)
                {
                    SetValue(InputProperty, brush);
                }
                else
                {
                    SetValue(InputProperty, new SolidColorBrush(Colors.Transparent));
                }
            }
        }

        public float Min
        {
            get => (float)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public float Max
        {
            get => (float)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
    }

    /// <summary>
    /// Xray ShaderEffect 클래스
    /// </summary>
    public class XrayShaderManager : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(XrayShaderManager), 0);

        public static readonly DependencyProperty TextureSizeProperty = DependencyProperty.Register("TextureSize", typeof(Size), typeof(XrayShaderManager), new UIPropertyMetadata(new Size(1f, 1f), PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty SharpnessLevelProperty = DependencyProperty.Register("SharpnessLevel", typeof(float), typeof(XrayShaderManager), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty BrightnessLevelProperty = DependencyProperty.Register("BrightnessLevel", typeof(float), typeof(XrayShaderManager), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty ContrastLevelProperty = DependencyProperty.Register("ContrastLevel", typeof(float), typeof(XrayShaderManager), new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(3)));

        public static readonly DependencyProperty ColorModeProperty = DependencyProperty.Register("ColorMode", typeof(float), typeof(XrayShaderManager), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(4)));

        private static readonly PixelShader _pixelShader = new()
        {
            UriSource = new Uri("/MZ.Resource;component/Shaders/xray.ps", UriKind.Relative)
        };

        public XrayShaderManager()
        {
            this.PixelShader = _pixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(TextureSizeProperty);
            UpdateShaderValue(SharpnessLevelProperty);
            UpdateShaderValue(BrightnessLevelProperty);
            UpdateShaderValue(ContrastLevelProperty);
            UpdateShaderValue(ColorModeProperty);
        }

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set
            {
                if (value == null)
                {
                    SetValue(InputProperty, new SolidColorBrush(Colors.Transparent));
                }
                else if (value is Brush brush)
                {
                    SetValue(InputProperty, brush);
                }
                else
                {
                    SetValue(InputProperty, new SolidColorBrush(Colors.Transparent));
                }
            }
        }

        public Size TextureSize
        {
            get => (Size)GetValue(TextureSizeProperty);
            set => SetValue(TextureSizeProperty, value);
        }

        public float SharpnessLevel
        {
            get => (float)GetValue(SharpnessLevelProperty);
            set => SetValue(SharpnessLevelProperty, value);
        }

        public float BrightnessLevel
        {
            get => (float)GetValue(BrightnessLevelProperty);
            set => SetValue(BrightnessLevelProperty, value);
        }

        public float ContrastLevel
        {
            get => (float)GetValue(ContrastLevelProperty);
            set => SetValue(ContrastLevelProperty, value);
        }

        public float ColorMode
        {
            get => (float)GetValue(ColorModeProperty);
            set => SetValue(ColorModeProperty, value);
        }
    }
}
