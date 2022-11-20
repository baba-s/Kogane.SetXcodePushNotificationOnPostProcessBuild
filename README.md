# Kogane Set Xcode Push Notification On Post Process Build

iOS ビルド完了時に Xcode プロジェクトのプッシュ通知を有効化するエディタ拡張

## 使用例

```cs
using Kogane;
using UnityEditor;

[InitializeOnLoad]
public static class Example
{
    static Example()
    {
        // 開発ビルドかリリースビルドか設定します
        SetXcodePushNotificationOnPostProcessBuild.OnIsProduction = () => false;
    }
}
```