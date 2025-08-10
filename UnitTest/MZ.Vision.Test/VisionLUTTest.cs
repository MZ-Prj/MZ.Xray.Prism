using OpenCvSharp;
using Xunit;
using static MZ.Vision.VisionEnums;
using Assert = Xunit.Assert;


namespace MZ.Vision.Test
{
    public class VisionLUTTest
    {
        [Fact]
        public void VisionLUTModel_Defaults_AreInitialized()
        {
            var model = new VisionLUTModel
            {
                Name = FunctionNameEnumTypes.Base
            };

            Assert.NotNull(model.Parameters);
            Assert.NotNull(model.MinMax);
            Assert.Equal(FunctionNameEnumTypes.Base, model.Name);

            model.Parameters.AddRange([1.0, 0.0, 0.0]);
            model.MinMax.Add(new Point(0, 0));
            Assert.Single(model.MinMax);
            Assert.Equal(3, model.Parameters.Count);
        }

        [Fact]
        public void Run_Dispatches_To_Base()
        {
            var a = 2.0; 
            var b = 0.5; 
            var c = -1.0;

            var y = VisionLUT.Run(FunctionNameEnumTypes.Base, input: 0.5, values: [a, b, c]);
            
            Assert.Equal(1.0, y, 6);
        }

        [Fact]
        public void Run_Dispatches_To_Pow()
        {
            var y = VisionLUT.Run(FunctionNameEnumTypes.Pow, input: 0.5, values: [2.0]);

            Assert.Equal(0.25, y, 6);
        }

        [Fact]
        public void Run_Dispatches_To_Log()
        {
            var a = 10.0;

            var y = VisionLUT.Run(FunctionNameEnumTypes.Log, input: 9.0, values: [a]);

            Assert.Equal(1.0, y, 6);
        }

        [Fact]
        public void Run_Dispatches_To_Atanh()
        {
            var y = VisionLUT.Run(FunctionNameEnumTypes.Atanh, input: 0.0, values: [1.0, 1.0, 1.0, 0.0]);
            Assert.Equal(0.5, y, 6);
        }

        [Fact]
        public void Base_Linear_Function_Works()
        {
            var y = VisionLUT.Base(2.0, [3.0, -2.0, 5.0]);

            Assert.Equal(5.0, y, 6);
        }

        [Fact]
        public void Atanh_Clamps_And_Normalizes_To_ZeroToOne()
        {

            var high = VisionLUT.Atanh(0.99, [5.0, 1.0, 1.0, 0.0]);
            Assert.Equal(1.0, high, 6);

            var low = VisionLUT.Atanh(-0.99, [5.0, 1.0, 1.0, 0.0]);
            Assert.Equal(0.0, low, 6);

            var mid = VisionLUT.Atanh(0.0, [1.0, 1.0, 1.0, 0.0]);
            Assert.Equal(0.5, mid, 6);
        }

        [Fact]
        public void Atanh_On_InvalidParams_ReturnsZero_ByErrorHandling()
        {
            var y = VisionLUT.Atanh(0.2, []);

            Assert.Equal(0.0, y, 6);
        }

        [Fact]
        public void Log_Base_One_Produces_PositiveInfinity_Then_ErrorCheck_ToOne()
        {
            var y = VisionLUT.Run(FunctionNameEnumTypes.Log, input: 1.0, values: [1.0]);

            Assert.Equal(1.0, y, 6);
        }

        [Fact]
        public void Log_NegativeBase_Produces_NaN_Then_ErrorCheck_ToOne()
        {
            var y = VisionLUT.Run(FunctionNameEnumTypes.Log, input: 0.5, values: [-2.0]);

            Assert.Equal(1.0, y, 6);
        }

        [Fact]
        public void Pow_ZeroExponent_ReturnsOne()
        {
            var y = VisionLUT.Run(FunctionNameEnumTypes.Pow, input: 0.12345, values: [0.0]);
            Assert.Equal(1.0, y, 6);
        }

        [Fact]
        public void ErrorCheck_Maps_Infinities_And_NaN_Properly()
        {
            var posInf = VisionLUT.Run(FunctionNameEnumTypes.Log, input: 9.0, values: [1.0]);
            Assert.Equal(1.0, posInf, 6);

            var nan = VisionLUT.Run(FunctionNameEnumTypes.Log, input: 0.0, values: [-3.0]);
            Assert.Equal(1.0, nan, 6);

            var negInfLike = VisionLUT.Run(FunctionNameEnumTypes.Atanh, input: -0.9999, values: [10.0, 1.0, 1.0, 0.0]);
            Assert.Equal(0.0, negInfLike, 6);
        }

        [Fact]
        public void Log_Domain_Starts_At_MinusOne_Inclusive()
        {
            var y = VisionLUT.Run(FunctionNameEnumTypes.Log, input: -1.0, values: new[] { 10.0 });
            
            Assert.Equal(0.0, y, 6);
        }
    }
}
