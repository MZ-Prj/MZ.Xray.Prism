wix extension add WixToolset.Util.wixext/6.0.1
wix build Product.wxs ^
  -arch x64 ^
  -ext WixToolset.UI.wixext ^
  -ext WixToolset.Util.wixext ^
  -d AppSource="D:\workspace\MZ.Xray.Prism\Application\Build\x64\Release\net8.0-windows7.0" ^
  -o AppSetup.msi