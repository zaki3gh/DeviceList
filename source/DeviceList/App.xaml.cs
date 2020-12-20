using MyApps.DeviceList.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 分割アプリケーション テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234228 を参照してください

namespace MyApps.DeviceList
{
    /// <summary>
    /// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
        /// 最初の行であり、main() または WinMain() と論理的に等価です。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            Common.SuspensionManager.KnownTypes.Add(typeof(GroupType));
        }

        /// <summary>
        /// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ ポイントは、
        /// アプリケーションが特定のファイルを開くために呼び出されたときに
        /// 検索結果やその他の情報を表示するために使用されます。
        /// </summary>
        /// <param name="args">起動要求とプロセスの詳細を表示します。</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
            // ウィンドウがアクティブであることだけを確認してください
            
            if (rootFrame == null)
            {
                // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
                rootFrame = new Frame();
                //フレームを SuspensionManager キーに関連付けます
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // 必要な場合のみ、保存されたセッション状態を復元します
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //状態の復元に何か問題があります。
                        //状態がないものとして続行します
                    }
                }

                // フレームを現在のウィンドウに配置します
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // ナビゲーション スタックが復元されていない場合、最初のページに移動します。
                // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
                // を構成します
                if (!rootFrame.Navigate(typeof(DeviceItemGroupedItemsPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // 現在のウィンドウがアクティブであることを確認します
            Window.Current.Activate();

            // Share Sourceとしての仕込み
            //  サンプルをまねしてPageインスタンス内のOnNavigatedToで+(アタッチ)し
            //  OnNavigatingFromで-(でタッチ)すると、中断->再開で共有できなくなる.
            //    (OnNavigatingFromのみ呼ばれOnNavigatedToが呼ばれないため)
            //  サンプルがその実装でよいのは、それぞれのPageインスタンスが
            //  MainPage内のFrameにホストされるもので、独立したページとして表示されるものではないため
            //  中断でOnNavigatingFromが呼び出されないため。
            Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        /// <summary>
        /// アプリケーションの実行が中断されたときに呼び出されます。アプリケーションの状態は、
        /// アプリケーションが終了されるのか、メモリの内容がそのままで再開されるのか
        /// わからない状態で保存されます。
        /// </summary>
        /// <param name="sender">中断要求の送信元。</param>
        /// <param name="e">中断要求の詳細。</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        ///  共有ソースとして<c>DataRequested</c>イベントを処理する.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnDataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            if (Window.Current == null)
            {
                return;
            }
            var frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }
            var page = frame.Content as IShareSourcePage;
            if (page == null)
            {
                return;
            }

            page.OnDataRequested(sender, args);
        }
    }
}
