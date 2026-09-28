// Warframe Market 最安値アラート / Undercut Alert  (C# / WinForms, .NET Framework 4.x)
// Windows標準のcsc.exeでコンパイルできます（C# 5互換）。build.bat を実行してください。
// Compiles with the csc.exe that ships with Windows (C# 5 compatible). Run build.bat.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WfmAlert
{
    // ===== 多言語 (ja / en) =====
    static class Loc
    {
        public static volatile string Lang = "en";
        static Dictionary<string, string[]> S = new Dictionary<string, string[]>();
        static void A(string k, string ja, string en) { S[k] = new string[] { ja, en }; }
        public static string T(string k)
        {
            string[] v;
            if (!S.TryGetValue(k, out v)) return k;
            return Lang == "ja" ? v[0] : v[1];
        }
        public static string F(string k, params object[] a) { return string.Format(T(k), a); }

        static Loc()
        {
            A("title", "Warframe Market 最安値アラート", "Warframe Market Undercut Alert");
            A("m_file", "ファイル(&F)", "&File");
            A("m_exit", "終了(&X)", "E&xit");
            A("m_clear_creds", "保存したログイン情報を削除", "Delete saved login");
            A("m_lang", "言語(&L)", "&Language");
            A("m_help", "ヘルプ(&H)", "&Help");
            A("m_usage", "使い方(&U)", "&How to use");
            A("m_about", "このツールについて(&A)", "&About");
            A("grp", "設定", "Settings");
            A("slug", "slug", "Slug");
            A("plat", "プラットフォーム", "Platform");
            A("intv", "チェック間隔(分)", "Check interval (min)");
            A("start", "自動チェック", "Auto check");
            A("stop", "停止", "Stop");
            A("now", "即時チェック", "Check once");
            A("vis_toggle", "公開状態を切り替え", "Toggle visibility");
            A("vis_all_public", "すべて公開", "Make all public");
            A("vis_all_private", "すべて非公開", "Make all private");
            A("vis_make_public", "公開にする", "Make public");
            A("vis_make_private", "非公開にする", "Make private");
            A("vis_confirm", "自分の売り注文すべてを{0}にします。よろしいですか？", "Make all your sell orders {0}?");
            A("vis_done", "公開状態を{0}に変更しました。", "Visibility changed to {0}.");
            A("vis_fail", "公開状態の変更に失敗しました: {0}", "Failed to change visibility: {0}");
            A("vis_target_public", "公開", "public");
            A("vis_target_private", "非公開", "private");
            A("price", "価格を変更…", "Change price...");
            A("my_status", "自分の状態", "My status");
            A("set_status", "状態を変更", "Set status");
            A("market_home", "Warframe.marketを開く", "Open Warframe.market");
            A("market_open_fail", "Warframe.marketを開けませんでした: {0}", "Could not open Warframe.market: {0}");
            A("status_done", "Warframe Marketの状態を「{0}」に1時間設定しました。", "Warframe Market status set to {0} for one hour.");
            A("status_invisible_done", "Warframe Marketの状態をInvisibleにしました（期限なし）。", "Warframe Market status set to Invisible with no expiry.");
            A("status_fail", "状態の変更に失敗しました: {0}", "Failed to change status: {0}");
            A("status_online", "Online", "Online");
            A("status_ingame", "In Game", "In Game");
            A("status_offline", "Offline", "Offline");
            A("status_invisible", "Invisible", "Invisible");
            A("st_idle", "停止中", "Stopped");
            A("st_run", "監視中…", "Monitoring...");
            A("st_last", "最終チェック {0}", "Last check {0}");
            A("c_item", "アイテム", "Item");
            A("c_rank", "ランク", "Rank");
            A("c_mine", "自分(p)", "Mine (p)");
            A("c_low", "他人の最安(p)", "Lowest other (p)");
            A("c_state", "状態", "Status");
            A("c_visibility", "公開状態", "Visibility");
            A("c_total_qty", "同一アイテム数量", "Item quantity");
            A("visibility_public", "公開", "Public");
            A("visibility_private", "非公開", "Private");
            A("visibility_unknown", "不明", "Unknown");
            A("s_under", "抜かれた", "Undercut");
            A("s_low", "最安", "Lowest");
            A("s_none", "他に出品なし", "No other sellers");
            A("l_start", "自動チェック開始（{0}分ごと）", "Auto check started (every {0} min)");
            A("l_stop", "停止しました", "Stopped");
            A("l_check_once", "即時チェック中…", "Checking now…");
            A("l_check_done", "即時チェック完了（{0}件）", "Immediate check complete ({0} listings)");
            A("l_err", "エラー: {0}", "Error: {0}");
            A("a_under", "⚠️ {0} (rank {1}) 自分 {2}p / 最安 {3}p", "⚠️ {0} (rank {1}) mine {2}p / lowest {3}p");
            A("a_back", "✅ {0} (rank {1}) 再び最安（{2}p）", "✅ {0} (rank {1}) lowest again ({2}p)");
            A("a_title", "WFM 最安値アラート", "WFM Undercut Alert");
            A("m_slug_req", "slug を入力してください。", "Please enter your slug.");
            A("m_start_first", "先に「自動チェック」を押してください。", "Press Auto check first.");
            A("t_open", "開く", "Open");
            A("t_exit", "終了", "Exit");
            A("t_hidden", "タスクトレイで監視を続けています", "Still monitoring in the system tray");
            A("p_select", "価格を変更する行を、一覧から選んでください。", "Select a row in the list first.");
            A("open_item", "warframe.marketでアイテムを開く", "Open item on warframe.market");
            A("lg_remember", "ログイン情報をこのPCに保存する（Windowsで暗号化）", "Save login on this PC (encrypted by Windows)");
            A("lg_cleared", "保存したログイン情報を削除しました。", "Saved login was deleted.");
            A("p_title", "価格を変更", "Change price");
            A("p_cur", "現在の価格", "Current price");
            A("p_low", "他人の最安値", "Lowest other");
            A("p_new", "新しい価格(p)", "New price (p)");
            A("p_noid", "この注文のIDが取得できていません。", "The order ID is not available.");
            A("p_done", "価格を変更しました: {0} → {1}p", "Price updated: {0} -> {1}p");
            A("p_fail", "価格の変更に失敗しました: {0}", "Failed to change price: {0}");
            A("ok", "OK", "OK");
            A("cancel", "キャンセル", "Cancel");
            A("lg_title", "warframe.market にログイン", "Log in to warframe.market");
            A("lg_email", "メールアドレス", "Email");
            A("lg_pass", "パスワード", "Password");
            A("lg_note",
              "チェックを入れるとログイン情報をWindowsのユーザー単位で暗号化して保存します。保存しない場合、ログイン状態はアプリを閉じると消えます。",
              "If checked, your login is encrypted and saved for your Windows user. Otherwise, the session is forgotten when you close the app.");
            A("lg_fail", "ログインに失敗しました: {0}", "Login failed: {0}");
            A("lg_expired", "ログインの有効期限が切れました。もう一度お試しください。", "Your login expired. Please try again.");
            A("h_title", "使い方", "How to use");
            A("h_text",
              string.Join("\r\n", new string[] {
                "■ このツールについて",
                "warframe.market に出している「売り注文」を定期的に見張り、他のプレイヤーに最安値を抜かれたらアプリ内ログに記録します。PCへの通知は行いません。",
                "",
                "■ はじめに",
                "1. slug に、あなたのwarframe.marketのユーザーID（slug）を入力します。IGNを小文字にしたものが多いです（例: TennoName → tennoname）。プロフィールページのURL末尾でも確認できます。",
                "2. チェック間隔（分）を決めて「自動チェック」を押します。「即時チェック」なら一度だけ確認できます。",
                "",
                "■ 一覧の見方",
                "自分の売り注文ごとに、価格、公開状態、同じアイテムの出品数量合計、ゲーム内（In Game）の他の出品者の最安値を表示します。他の人のほうが安い行は赤くなり、状態が「抜かれた」になります。非公開注文もログイン後に表示されます。",
                "",
                "■ 価格の変更",
                "一覧の行をダブルクリックして新しい価格を入力します。抜かれている場合は、ゲーム内の他人の最低価格が最初に入力されます。行を右クリックすると、そのアイテムのwarframe.marketページを開けます。",
                "公開状態は行を右クリックして切り替えられます。上部の「すべて公開」「すべて非公開」は、自分の売り注文すべてに適用されます。",
                "初回は、warframe.marketのメールアドレスとパスワードでログインします（メールとパスワードで登録したアカウントのみ）。ログイン画面で保存にチェックすると、ログイン情報はWindowsのユーザー単位で暗号化して保存され、次回から自動で使われます。保存情報は「ファイル」メニューから削除できます。",
                "※この機能は、まだ正式提供されていない旧方式（v1）のログインを使います。仕様変更で使えなくなる可能性があります。",
                "",
                "■ その他",
                "・監視中にウィンドウの×を押すと、終了せずタスクトレイに隠れます。完全に終了するには、トレイのアイコンを右クリックして「終了」を選びます。",
                "・APIの利用制限（1秒あたり3回まで）を守るため、注文が多いとチェックに時間がかかります。",
                "・言語は「言語」メニューから切り替えられます。",
              }),
              string.Join("\r\n", new string[] {
                "■ About this tool",
                "It watches your sell orders on warframe.market and records undercuts in the in-app log. It does not send PC notifications.",
                "",
                "■ Getting started",
                "1. Enter your warframe.market user ID (slug) in Slug. It is often your in-game name in lowercase (e.g. TennoName -> tennoname). You can also find it at the end of your profile page URL.",
                "2. Choose the check interval (minutes) and press Auto check. Use Check once to run a one-time check.",
                "",
                "■ Reading the list",
                "For each of your sell orders, the list shows price, visibility, the total quantity listed for that item, and the lowest price among other sellers who are In Game. Private orders appear after sign-in.",
                "",
                "■ Changing a price",
                "Double-click a row to enter a new price. If you are undercut, the lowest other In Game price is filled in. Right-click a row to open its warframe.market item page.",
                "Right-click a row to toggle its visibility. Make all public/private applies to all of your sell orders.",
                "The first time, log in with your warframe.market email and password (email/password accounts only). If you check Save login, Windows encrypts and stores the credentials for your Windows user, then the app uses them automatically. Use the File menu to delete saved credentials.",
                "Note: this feature uses the old (v1) login, which is not officially supported anymore. It may stop working if the service changes.",
                "",
                "■ Other notes",
                "- If you press the window's X button while monitoring, the app hides in the system tray instead of quitting. To quit completely, right-click the tray icon and choose Exit.",
                "- To respect the API rate limit (3 requests per second), checks take longer when you have many orders.",
                "- You can switch the language from the Language menu.",
              }));
            A("ab_text",
              "Warframe Market 最安値アラート\r\n\r\n非公式のファンメイドツールです。warframe.market、Digital Extremesとは関係ありません。",
              "Warframe Market Undercut Alert\r\n\r\nAn unofficial fan-made tool. Not affiliated with warframe.market or Digital Extremes.");
        }
    }

    static class CredentialProtection
    {
        static readonly byte[] Entropy = Encoding.UTF8.GetBytes("WfmAlert.credentials.v1");

        public static string Protect(string email, string password)
        {
            byte[] plain = Encoding.UTF8.GetBytes(email + "\0" + password);
            byte[] protectedData = ProtectedData.Protect(plain, Entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedData);
        }

        public static string[] Unprotect(string value)
        {
            byte[] protectedData = Convert.FromBase64String(value);
            byte[] plain = ProtectedData.Unprotect(protectedData, Entropy, DataProtectionScope.CurrentUser);
            string[] fields = Encoding.UTF8.GetString(plain).Split('\0');
            if (fields.Length != 2) throw new FormatException("Invalid saved credentials");
            return fields;
        }
    }

    class Config
    {
        public string Slug { get; set; }
        public string ProtectedCredentials { get; set; }
        public int Interval { get; set; }
        public string Platform { get; set; }
        public bool Sound { get; set; } // legacy config compatibility; notifications are disabled
        public string Lang { get; set; }
        public Config()
        {
            Slug = ""; Interval = 5; Platform = "pc"; Sound = true;
            Lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja" ? "ja" : "en";
        }
    }

    enum RowState { Under, Lowest, NoOthers }

    class Row
    {
        public string Item, ItemId, OrderId;
        public int? RankValue, ChargesValue, AmberStarsValue, CyanStarsValue, PerTradeValue, LowValue;
        public string SubtypeValue;
        public int MineValue, Quantity, SameItemQuantity;
        public bool? Visible;
        public RowState State;
        public string RankText { get { return RankValue.HasValue ? RankValue.Value.ToString() : "-"; } }
        public string Key { get { return Item + "|" + RankText + "|" + (ChargesValue.HasValue ? ChargesValue.Value.ToString() : "-") + "|" + (SubtypeValue ?? "-") + "|" + (AmberStarsValue.HasValue ? AmberStarsValue.Value.ToString() : "-") + "|" + (CyanStarsValue.HasValue ? CyanStarsValue.Value.ToString() : "-") + "|" + (PerTradeValue.HasValue ? PerTradeValue.Value.ToString() : "-"); } }
    }

    // ===== 公開API v2（読み取り専用・ログイン不要）=====
    class Api
    {
        public const string BaseUrl = "https://api.warframe.market/v2";
        public string Slug;
        string platform;
        volatile string authToken;
        JavaScriptSerializer json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };

        public Api(string slug, string platform, string token)
        {
            Slug = slug.Trim().ToLowerInvariant();
            this.platform = platform;
            authToken = token;
        }

        public object Get(string path, string query)
        {
            Thread.Sleep(400); // 1秒3回の制限に余裕を持たせる
            var req = (HttpWebRequest)WebRequest.Create(BaseUrl + path + query);
            // 公式ルールで識別できるUser-Agentが求められています
            req.UserAgent = "WfmAlert/1.0 (unofficial Warframe Market listing monitor)";
            req.Accept = "application/json";
            req.Headers["Platform"] = platform;
            req.Headers["Language"] = "en";
            if (!string.IsNullOrEmpty(authToken))
            {
                string bearer = authToken.Trim();
                if (bearer.StartsWith("JWT ", StringComparison.OrdinalIgnoreCase)) bearer = bearer.Substring(4).Trim();
                else if (bearer.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) bearer = bearer.Substring(7).Trim();
                req.Headers["Authorization"] = "Bearer " + bearer;
            }
            req.Timeout = 15000;
            string body;
            using (var res = (HttpWebResponse)req.GetResponse())
            using (var sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                body = sr.ReadToEnd();
            var root = json.DeserializeObject(body) as Dictionary<string, object>;
            if (root == null) throw new Exception("Unexpected response");
            object err;
            if (root.TryGetValue("error", out err) && err != null)
                throw new Exception("API error: " + json.Serialize(err));
            object data;
            return root.TryGetValue("data", out data) ? data : null;
        }

        public static object Field(object o, string key)
        {
            var d = o as Dictionary<string, object>;
            object v;
            if (d != null && d.TryGetValue(key, out v)) return v;
            return null;
        }

        public static int IntOr(object v, int def) { return v == null ? def : Convert.ToInt32(v); }

        public Dictionary<string, string> LoadItems()
        {
            var map = new Dictionary<string, string>();
            var arr = Get("/items", "") as IEnumerable;
            if (arr != null)
                foreach (object it in arr)
                {
                    string id = Field(it, "id") as string;
                    string slug = Field(it, "slug") as string;
                    if (id != null && slug != null) map[id] = slug;
                }
            return map;
        }

        public List<object> LoadOwnOrders()
        {
            string path = string.IsNullOrEmpty(authToken)
                ? "/orders/user/" + Uri.EscapeDataString(Slug)
                : "/orders/my";
            var result = Get(path, "") as IEnumerable;
            if (result == null) throw new Exception("Unexpected user-orders response");
            var orders = new List<object>();
            foreach (object order in result) orders.Add(order);
            return orders;
        }

        // 公開注文を全件取得し、同じ仕様のオンライン売り注文から最安値を求める。
        public List<object> LoadItemOrders(string item)
        {
            var result = Get("/orders/item/" + Uri.EscapeDataString(item), "") as IEnumerable;
            if (result == null) throw new Exception("Unexpected item-order response");
            var orders = new List<object>();
            foreach (object o in result) orders.Add(o);
            return orders;
        }

        static int? OptionalInt(object value)
        {
            return value == null ? (int?)null : Convert.ToInt32(value);
        }

        public int? LowestOther(IEnumerable orders, Row match)
        {
            int? low = null;
            foreach (object o in orders)
            {
                if ((Field(o, "type") as string) != "sell") continue;
                if (OptionalInt(Field(o, "rank")) != match.RankValue) continue;
                if (OptionalInt(Field(o, "charges")) != match.ChargesValue) continue;
                if (!string.Equals(Field(o, "subtype") as string, match.SubtypeValue, StringComparison.Ordinal)) continue;
                if (OptionalInt(Field(o, "amberStars")) != match.AmberStarsValue) continue;
                if (OptionalInt(Field(o, "cyanStars")) != match.CyanStarsValue) continue;
                if (OptionalInt(Field(o, "perTrade")) != match.PerTradeValue) continue;

                object user = Field(o, "user");
                string uslug = Field(user, "slug") as string;
                string status = Field(user, "status") as string;
                if (string.Equals(uslug, Slug, StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.Equals(status == null ? null : status.Trim(), "ingame", StringComparison.OrdinalIgnoreCase)) continue;
                int p = Convert.ToInt32(Field(o, "platinum"));
                if (!low.HasValue || p < low.Value) low = p;
            }
            return low;
        }
    }

    // ===== ログイン・価格変更（旧v1方式。公式には未サポート）=====
    static class Auth
    {
        public const string V1 = "https://api.warframe.market/v1";

        static HttpWebRequest NewReq(string url, string method, string platform)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.UserAgent = "WfmAlert/1.0 (unofficial Warframe Market listing monitor)";
            req.Accept = "application/json";
            req.ContentType = "application/json";
            req.Headers["Platform"] = platform;
            req.Headers["Language"] = "en";
            req.Timeout = 15000;
            return req;
        }

        static void Send(HttpWebRequest req, Dictionary<string, object> payload)
        {
            byte[] data = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(payload));
            using (var s = req.GetRequestStream()) s.Write(data, 0, data.Length);
        }

        // 戻り値: "JWT xxxxx"（Authorizationヘッダーに使う文字列）
        public static string SignIn(string email, string password, string platform)
        {
            var req = NewReq(V1 + "/auth/signin", "POST", platform);
            req.Headers["Authorization"] = "JWT";
            var p = new Dictionary<string, object>();
            p["email"] = email; p["password"] = password; p["auth_type"] = "header";
            Send(req, p);
            using (var res = (HttpWebResponse)req.GetResponse())
            {
                string token = res.Headers["Authorization"];
                if (string.IsNullOrEmpty(token)) throw new Exception("No token in response");
                return token;
            }
        }

        public static void UpdateOrder(string token, Row r, int platinum, string platform)
        {
            var req = NewReq(Api.BaseUrl + "/order/" + Uri.EscapeDataString(r.OrderId), "PATCH", platform);
            string accessToken = token.Trim();
            if (accessToken.StartsWith("JWT ", StringComparison.OrdinalIgnoreCase))
                accessToken = accessToken.Substring(4).Trim();
            else if (accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                accessToken = accessToken.Substring(7).Trim();
            req.Headers["Authorization"] = "Bearer " + accessToken;
            var p = new Dictionary<string, object>();
            p["platinum"] = platinum;
            Send(req, p);
            using (req.GetResponse()) { }
        }

        public static void UpdateVisibility(string token, string orderId, bool visible, string platform)
        {
            var req = NewReq(Api.BaseUrl + "/order/" + Uri.EscapeDataString(orderId), "PATCH", platform);
            req.Headers["Authorization"] = "Bearer " + CleanToken(token);
            var p = new Dictionary<string, object>(); p["visible"] = visible;
            Send(req, p);
            using (req.GetResponse()) { }
        }

        public static void UpdateAllSellVisibility(string token, bool visible, string platform)
        {
            var req = NewReq(Api.BaseUrl + "/orders/group/all", "PATCH", platform);
            req.Headers["Authorization"] = "Bearer " + CleanToken(token);
            var p = new Dictionary<string, object>(); p["visible"] = visible; p["type"] = "sell";
            Send(req, p);
            using (req.GetResponse()) { }
        }

        static string CleanToken(string token)
        {
            string t = (token ?? "").Trim();
            if (t.StartsWith("JWT ", StringComparison.OrdinalIgnoreCase)) t = t.Substring(4).Trim();
            else if (t.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) t = t.Substring(7).Trim();
            return t;
        }

        static Dictionary<string, object> ReceiveWsMessage(ClientWebSocket ws, CancellationToken ct)
        {
            var buffer = new byte[4096];
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    result = ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct).GetAwaiter().GetResult();
                    if (result.MessageType == WebSocketMessageType.Close) throw new Exception("WebSocket closed by server");
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);
                return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Encoding.UTF8.GetString(ms.ToArray()));
            }
        }

        static string WsRoute(Dictionary<string, object> msg)
        {
            object route;
            return msg != null && msg.TryGetValue("route", out route) ? route as string : null;
        }

        static string WsError(Dictionary<string, object> msg)
        {
            object payload;
            if (msg != null && msg.TryGetValue("payload", out payload))
            {
                var data = payload as Dictionary<string, object>;
                object error;
                if (data != null && data.TryGetValue("error", out error)) return Convert.ToString(error);
                return Convert.ToString(payload);
            }
            return "Unknown WebSocket error";
        }

        public static void SetUserStatus(string token, string status)
        {
            using (var ws = new ClientWebSocket())
            using (var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15)))
            {
                ws.Options.AddSubProtocol("wfm");
                ws.ConnectAsync(new Uri("wss://ws.warframe.market/socket"), timeout.Token).GetAwaiter().GetResult();

                string id = Guid.NewGuid().ToString("N").Substring(0, 11);
                var login = new Dictionary<string, object>();
                login["route"] = "@wfm|cmd/auth/signIn";
                login["id"] = id;
                login["payload"] = new Dictionary<string, object> { { "token", CleanToken(token) } };
                SendWs(ws, login, timeout.Token);

                bool authenticated = false;
                DateTime until = DateTime.UtcNow.AddSeconds(8);
                while (DateTime.UtcNow < until)
                {
                    Dictionary<string, object> msg = ReceiveWsMessage(ws, timeout.Token);
                    string route = WsRoute(msg) ?? "";
                    if (route.StartsWith("@wfm|cmd/auth/signIn:error", StringComparison.Ordinal)) throw new Exception(WsError(msg));
                    if (route.StartsWith("@wfm|cmd/auth/signIn:ok", StringComparison.Ordinal)) { authenticated = true; break; }
                }
                if (!authenticated) throw new TimeoutException("WebSocket authentication timed out");

                var set = new Dictionary<string, object>();
                set["route"] = "@wfm|cmd/status/set";
                set["id"] = Guid.NewGuid().ToString("N").Substring(0, 11);
                var statusPayload = new Dictionary<string, object> { { "status", status } };
                if (status == "online" || status == "ingame") statusPayload["duration"] = 3600;
                set["payload"] = statusPayload;
                SendWs(ws, set, timeout.Token);

                until = DateTime.UtcNow.AddSeconds(8);
                while (DateTime.UtcNow < until)
                {
                    Dictionary<string, object> msg = ReceiveWsMessage(ws, timeout.Token);
                    string route = WsRoute(msg) ?? "";
                    if (route.StartsWith("@wfm|cmd/status/set:error", StringComparison.Ordinal)) throw new Exception(WsError(msg));
                    if (route.StartsWith("@wfm|cmd/status/set:ok", StringComparison.Ordinal) ||
                        route.StartsWith("@wfm|event/status/set", StringComparison.Ordinal)) return;
                }
                throw new TimeoutException("Status update timed out");
            }
        }

        static void SendWs(ClientWebSocket ws, Dictionary<string, object> msg, CancellationToken ct)
        {
            byte[] data = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(msg));
            ws.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, ct).GetAwaiter().GetResult();
        }


        public static int StatusCode(Exception e)
        {
            var we = e as WebException;
            var hr = we == null ? null : we.Response as HttpWebResponse;
            return hr == null ? 0 : (int)hr.StatusCode;
        }

        public static string Describe(Exception e)
        {
            var we = e as WebException;
            if (we != null && we.Response != null)
            {
                string body = "";
                try { using (var sr = new StreamReader(we.Response.GetResponseStream(), Encoding.UTF8)) body = sr.ReadToEnd(); }
                catch (Exception) { }
                if (body.Length > 200) body = body.Substring(0, 200);
                return (StatusCode(e) + " " + body).Trim();
            }
            return e.Message;
        }
    }

    // ===== 監視スレッド =====
    class Monitor
    {
        Config cfg;
        Action<string> onLog;
        Action<List<Row>> onRows;
        string authToken;
        volatile bool stop;
        ManualResetEvent wake = new ManualResetEvent(false);
        Dictionary<string, string> items = new Dictionary<string, string>();
        Dictionary<string, int> alerted = new Dictionary<string, int>(); // 通知済みの他人の最安値
        Thread th;

        public Monitor(Config c, string token, Action<string> log, Action<List<Row>> rows)
        {
            cfg = c; authToken = token; onLog = log; onRows = rows;
        }

        public bool Running { get { return th != null && th.IsAlive; } }
        public void Start() { th = new Thread(Run); th.IsBackground = true; th.Start(); }
        public void Stop() { stop = true; wake.Set(); }
        public void CheckNow() { wake.Set(); }
        public void SetToken(string token) { authToken = token; }

        public static List<Row> CheckOnce(Config c, string token, Action<string> log)
        {
            var oneShot = new Monitor(c, token, log, delegate(List<Row> rows) { });
            return oneShot.Check(new Api(c.Slug, c.Platform, token));
        }

        void Run()
        {
            while (!stop)
            {
                try { onRows(Check(new Api(cfg.Slug, cfg.Platform, authToken))); }
                catch (Exception e) { onLog(Loc.F("l_err", Auth.Describe(e))); }
                wake.WaitOne(cfg.Interval * 60 * 1000);
                wake.Reset();
            }
        }

        List<Row> Check(Api api)
        {
            var rows = new List<Row>();
            var orders = api.LoadOwnOrders();
            var marketByItem = new Dictionary<string, List<object>>();
            foreach (object o in orders)
            {
                if ((Api.Field(o, "type") as string) != "sell") continue;
                object vis = Api.Field(o, "visible");
                string itemId = Api.Field(o, "itemId") as string;
                if (itemId == null) continue;
                if (!items.ContainsKey(itemId)) items = api.LoadItems();
                string item;
                if (!items.TryGetValue(itemId, out item)) continue;

                var row = new Row();
                row.ItemId = itemId;
                row.Item = item;
                row.OrderId = Api.Field(o, "id") as string;
                object rk = Api.Field(o, "rank");
                row.RankValue = rk == null ? (int?)null : Convert.ToInt32(rk);
                object charges = Api.Field(o, "charges");
                row.ChargesValue = charges == null ? (int?)null : Convert.ToInt32(charges);
                row.SubtypeValue = Api.Field(o, "subtype") as string;
                object amber = Api.Field(o, "amberStars");
                row.AmberStarsValue = amber == null ? (int?)null : Convert.ToInt32(amber);
                object cyan = Api.Field(o, "cyanStars");
                row.CyanStarsValue = cyan == null ? (int?)null : Convert.ToInt32(cyan);
                object perTrade = Api.Field(o, "perTrade");
                row.PerTradeValue = perTrade == null ? (int?)null : Convert.ToInt32(perTrade);
                row.MineValue = Convert.ToInt32(Api.Field(o, "platinum"));
                row.Quantity = Api.IntOr(Api.Field(o, "quantity"), 1);
                row.Visible = vis is bool ? (bool?)vis : null;
                List<object> itemOrders;
                if (!marketByItem.TryGetValue(item, out itemOrders))
                {
                    itemOrders = api.LoadItemOrders(item);
                    marketByItem[item] = itemOrders;
                }
                row.LowValue = api.LowestOther(itemOrders, row);

                int prev;
                if (row.LowValue.HasValue && row.LowValue.Value < row.MineValue)
                {
                    row.State = RowState.Under;
                    // 最安値が変わったときだけ通知（連投防止）
                    if (!alerted.TryGetValue(row.Key, out prev) || prev != row.LowValue.Value)
                    {
                        Notify(Loc.F("a_under", item, row.RankText, row.MineValue, row.LowValue.Value));
                        alerted[row.Key] = row.LowValue.Value;
                    }
                }
                else
                {
                    row.State = row.LowValue.HasValue ? RowState.Lowest : RowState.NoOthers;
                    if (alerted.Remove(row.Key))
                        Notify(Loc.F("a_back", item, row.RankText, row.MineValue));
                }
                rows.Add(row);
            }
            var listingsByItem = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (Row row in rows)
            {
                int count;
                listingsByItem.TryGetValue(row.ItemId, out count);
                listingsByItem[row.ItemId] = count + row.Quantity;
            }
            foreach (Row row in rows) row.SameItemQuantity = listingsByItem[row.ItemId];
            return rows;
        }

        void Notify(string msg)
        {
            onLog(msg);
        }
    }

    // ===== ダイアログ類 =====
    static class Dlg
    {
        public static Form Base(string title)
        {
            var f = new Form();
            f.Text = title;
            f.Font = SystemFonts.MessageBoxFont;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.StartPosition = FormStartPosition.CenterParent;
            f.MaximizeBox = false; f.MinimizeBox = false; f.ShowInTaskbar = false;
            f.AutoSize = true; f.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            f.Padding = new Padding(10);
            return f;
        }

        public static Label L(string t)
        {
            return new Label { Text = t, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 10, 3) };
        }

        public static FlowLayoutPanel Buttons(Form f)
        {
            var ok = new Button { Text = Loc.T("ok"), DialogResult = DialogResult.OK, AutoSize = true };
            var cancel = new Button { Text = Loc.T("cancel"), DialogResult = DialogResult.Cancel, AutoSize = true };
            f.AcceptButton = ok; f.CancelButton = cancel;
            var p = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
            p.Controls.Add(cancel); p.Controls.Add(ok);
            return p;
        }
    }

    class PriceDialog : Form
    {
        NumericUpDown num;
        public int Price { get { return (int)num.Value; } }

        public PriceDialog(Row r, int suggested)
        {
            Text = Loc.T("p_title");
            Font = SystemFonts.MessageBoxFont;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false; MinimizeBox = false; ShowInTaskbar = false;
            AutoSize = true; AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(10);

            num = new NumericUpDown { Minimum = 1, Maximum = 999999, Value = Math.Max(1, Math.Min(999999, suggested)), Width = 100 };
            var g = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Dock = DockStyle.Fill };
            g.Controls.Add(Dlg.L(Loc.T("c_item")), 0, 0); g.Controls.Add(Dlg.L(r.Item + (r.RankValue.HasValue ? " (rank " + r.RankValue.Value + ")" : "")), 1, 0);
            g.Controls.Add(Dlg.L(Loc.T("p_cur")), 0, 1); g.Controls.Add(Dlg.L(r.MineValue + "p"), 1, 1);
            g.Controls.Add(Dlg.L(Loc.T("p_low")), 0, 2); g.Controls.Add(Dlg.L(r.LowValue.HasValue ? r.LowValue.Value + "p" : "-"), 1, 2);
            g.Controls.Add(Dlg.L(Loc.T("p_new")), 0, 3); g.Controls.Add(num, 1, 3);
            var b = Dlg.Buttons(this);
            g.Controls.Add(b, 0, 4); g.SetColumnSpan(b, 2);
            Controls.Add(g);
        }
    }

    class LoginDialog : Form
    {
        TextBox email, pass;
        CheckBox remember;
        public string Email { get { return email.Text.Trim(); } }
        public string Password { get { return pass.Text; } }
        public bool RememberCredentials { get { return remember.Checked; } }

        public LoginDialog()
        {
            Text = Loc.T("lg_title");
            Font = SystemFonts.MessageBoxFont;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false; MinimizeBox = false; ShowInTaskbar = false;
            AutoSize = true; AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(10);

            email = new TextBox { Width = 260 };
            pass = new TextBox { Width = 260, UseSystemPasswordChar = true };
            remember = new CheckBox { Text = Loc.T("lg_remember"), Checked = true, AutoSize = true };
            var note = new Label { Text = Loc.T("lg_note"), AutoSize = true, MaximumSize = new Size(380, 0), Margin = new Padding(3, 8, 3, 8) };
            var g = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Dock = DockStyle.Fill };
            g.Controls.Add(Dlg.L(Loc.T("lg_email")), 0, 0); g.Controls.Add(email, 1, 0);
            g.Controls.Add(Dlg.L(Loc.T("lg_pass")), 0, 1); g.Controls.Add(pass, 1, 1);
            g.Controls.Add(remember, 0, 2); g.SetColumnSpan(remember, 2);
            g.Controls.Add(note, 0, 3); g.SetColumnSpan(note, 2);
            var b = Dlg.Buttons(this);
            g.Controls.Add(b, 0, 4); g.SetColumnSpan(b, 2);
            Controls.Add(g);
        }
    }

    class HelpForm : Form
    {
        public HelpForm(string title, string text)
        {
            Text = title;
            Font = SystemFonts.MessageBoxFont;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(640, 520);
            ShowInTaskbar = false;
            var tb = new TextBox
            {
                Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill, Text = text, BackColor = SystemColors.Window, BorderStyle = BorderStyle.None
            };
            Padding = new Padding(8);
            Controls.Add(tb);
            tb.Select(0, 0);
        }
    }

    // ===== メイン画面 =====
    class MainForm : Form
    {
        static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WFMAlert", "config.json");

        Monitor monitor;
        bool reallyExit;
        bool priceUpdateRunning;
        bool visibilityUpdateRunning;
        string jwt;                 // ログイン状態（メモリ上のみ。保存しない）
        string savedEmail, savedPassword, protectedCredentials;
        List<Row> lastRows;
        DateTime? lastCheck;

        TextBox txtSlug, txtLog;
        ComboBox cmbPlat;
        NumericUpDown numInt;
        Button btnStart, btnNow, btnSetStatus, btnMarket, btnAllPublic, btnAllPrivate;
        ComboBox cmbStatus;
        Label lblStatus, lblSlug, lblPlat, lblInt;
        GroupBox box;
        ListView list;
        NotifyIcon tray;
        ToolStripMenuItem miFile, miExit, miClearCredentials, miLang, miJa, miEn, miHelp, miUsage, miAbout, trayOpen, trayExit;
        ContextMenuStrip itemMenu;
        ToolStripMenuItem miOpenItem, miToggleVisibility;

        public MainForm()
        {
            Font = SystemFonts.MessageBoxFont;
            ClientSize = new Size(780, 580);
            MinimumSize = new Size(660, 460);
            Icon = LoadAppIcon();

            Config cfg = LoadConfig();
            Loc.Lang = cfg.Lang == "ja" ? "ja" : "en";
            protectedCredentials = cfg.ProtectedCredentials;
            if (!string.IsNullOrEmpty(protectedCredentials))
            {
                try
                {
                    string[] creds = CredentialProtection.Unprotect(protectedCredentials);
                    savedEmail = creds[0]; savedPassword = creds[1];
                }
                catch (Exception) { protectedCredentials = null; savedEmail = null; savedPassword = null; }
            }

            // 一覧
            list = new ListView { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, GridLines = true, HideSelection = false, MultiSelect = false };
            for (int i = 0; i < 7; i++) list.Columns.Add("", new int[] { 220, 45, 65, 100, 85, 75, 120 }[i]);
            list.DoubleClick += OnChangePrice;
            list.MouseDown += OnListMouseDown;
            itemMenu = new ContextMenuStrip();
            miOpenItem = new ToolStripMenuItem();
            miOpenItem.Click += delegate { OpenSelectedItemPage(); };
            miToggleVisibility = new ToolStripMenuItem();
            miToggleVisibility.Click += delegate { OnToggleSelectedVisibility(); };
            itemMenu.Opening += delegate(object sender, System.ComponentModel.CancelEventArgs e)
            {
                if (list.SelectedItems.Count == 0) { e.Cancel = true; return; }
                Row selected = list.SelectedItems[0].Tag as Row;
                miToggleVisibility.Text = selected != null && selected.Visible == true ? Loc.T("vis_make_private") : Loc.T("vis_make_public");
                miToggleVisibility.Enabled = selected != null && selected.Visible.HasValue && !visibilityUpdateRunning;
            };
            itemMenu.Items.Add(miOpenItem);
            itemMenu.Items.Add(miToggleVisibility);
            list.ContextMenuStrip = itemMenu;

            // ログ
            txtLog = new TextBox { Dock = DockStyle.Bottom, Height = 120, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };

            // ボタン行
            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(6, 4, 6, 4) };
            btnStart = new Button { AutoSize = true };
            btnNow = new Button { AutoSize = true };
            btnAllPublic = new Button { AutoSize = true, Text = Loc.T("vis_all_public") };
            btnAllPrivate = new Button { AutoSize = true, Text = Loc.T("vis_all_private") };
            btnAllPublic.Click += delegate { OnSetAllVisibility(true); };
            btnAllPrivate.Click += delegate { OnSetAllVisibility(false); };
            btnMarket = new Button {
                Text = Loc.T("market_home"),
                Width = 220, Height = 54,
                Image = CreateMarketIcon(),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(12, 4, 12, 4),
                Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(34, 48, 57),
                ForeColor = Color.FromArgb(89, 220, 199),
                Cursor = Cursors.Hand
            };
            btnMarket.FlatAppearance.BorderColor = Color.FromArgb(65, 143, 139);
            btnMarket.FlatAppearance.BorderSize = 1;
            btnMarket.Click += delegate { OpenMarketHome(); };
            btnSetStatus = new Button { AutoSize = true };
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 105 };
            cmbStatus.Items.AddRange(new object[] { Loc.T("status_ingame"), Loc.T("status_online"), Loc.T("status_invisible") });
            cmbStatus.SelectedIndex = 0;
            lblStatus = new Label { AutoSize = true, Margin = new Padding(12, 8, 0, 0) };
            btnStart.Click += OnToggle;
            btnNow.Click += delegate
            {
                if (monitor != null && monitor.Running) monitor.CheckNow();
                else
                {
                    Config c = CollectConfig();
                    if (c.Slug.Length == 0) { MessageBox.Show(this, Loc.T("m_slug_req"), Text); return; }
                    string token = jwt, email = null, password = null;
                    bool remember = false, fromSaved = false;
                    if (!GetLoginInfo(ref token, ref email, ref password, ref remember, ref fromSaved)) return;
                    btnNow.Enabled = false;
                    AddLog(Loc.T("l_check_once"));
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        try
                        {
                            if (token == null) token = Auth.SignIn(email, password, c.Platform);
                            string sessionToken = token;
                            UI(() => jwt = sessionToken);
                            SaveRememberedLogin(email, password, remember);
                            List<Row> rows = Monitor.CheckOnce(c, sessionToken,
                                msg => UI(() => AddLog(msg)));
                            UI(() => { ShowRows(rows); AddLog(Loc.F("l_check_done", rows.Count)); });
                        }
                        catch (Exception ex)
                        {
                            string msg = Loc.F("l_err", Auth.Describe(ex));
                            UI(() =>
                            {
                                if (fromSaved && Auth.StatusCode(ex) == 401) ClearSavedCredentials();
                                AddLog(msg);
                                MessageBox.Show(this, msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            });
                        }
                        finally { UI(() => btnNow.Enabled = true); }
                    });
                }
            };
            btnSetStatus.Click += OnSetStatus;
            bar.Controls.Add(btnStart); bar.Controls.Add(btnNow);
            bar.Controls.Add(btnAllPublic); bar.Controls.Add(btnAllPrivate);
            bar.Controls.Add(cmbStatus); bar.Controls.Add(btnSetStatus); bar.Controls.Add(btnMarket); bar.Controls.Add(lblStatus);

            // 設定
            box = new GroupBox { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };
            var grid = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 4 };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            txtSlug = new TextBox { Text = cfg.Slug, Dock = DockStyle.Fill };
            cmbPlat = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            cmbPlat.Items.AddRange(new object[] { "pc", "ps4", "xbox", "switch", "mobile" });
            cmbPlat.SelectedItem = cfg.Platform; if (cmbPlat.SelectedIndex < 0) cmbPlat.SelectedIndex = 0;
            numInt = new NumericUpDown { Minimum = 1, Maximum = 60, Value = Math.Max(1, Math.Min(60, cfg.Interval)), Width = 70 };
            lblSlug = Dlg.L(""); lblPlat = Dlg.L(""); lblInt = Dlg.L("");
            grid.Controls.Add(lblSlug, 0, 0); grid.Controls.Add(txtSlug, 1, 0);
            grid.Controls.Add(lblPlat, 2, 0); grid.Controls.Add(cmbPlat, 3, 0);
            grid.Controls.Add(lblInt, 0, 1); grid.Controls.Add(numInt, 1, 1);
            box.Controls.Add(grid);

            // メニューバー
            var menu = new MenuStrip();
            miFile = new ToolStripMenuItem(); miExit = new ToolStripMenuItem(); miClearCredentials = new ToolStripMenuItem();
            miLang = new ToolStripMenuItem(); miJa = new ToolStripMenuItem("日本語"); miEn = new ToolStripMenuItem("English");
            miHelp = new ToolStripMenuItem(); miUsage = new ToolStripMenuItem(); miAbout = new ToolStripMenuItem();
            miExit.Click += delegate { reallyExit = true; Close(); };
            miClearCredentials.Click += delegate { ClearSavedCredentials(); AddLog(Loc.T("lg_cleared")); };
            miJa.Click += delegate { SetLang("ja"); };
            miEn.Click += delegate { SetLang("en"); };
            miUsage.Click += delegate { using (var h = new HelpForm(Loc.T("h_title"), Loc.T("h_text"))) h.ShowDialog(this); };
            miAbout.Click += delegate { MessageBox.Show(this, Loc.T("ab_text"), Loc.T("title")); };
            miFile.DropDownItems.Add(miExit);
            miFile.DropDownItems.Add(miClearCredentials);
            miLang.DropDownItems.Add(miJa); miLang.DropDownItems.Add(miEn);
            miHelp.DropDownItems.Add(miUsage); miHelp.DropDownItems.Add(miAbout);
            menu.Items.Add(miFile); menu.Items.Add(miLang); menu.Items.Add(miHelp);
            MainMenuStrip = menu;

            // 追加順に注意: 最後に追加したものが一番外側（上）に配置される
            Controls.Add(list);
            Controls.Add(txtLog);
            Controls.Add(bar);
            Controls.Add(box);
            Controls.Add(menu);

            // タスクトレイ
            var tmenu = new ContextMenuStrip();
            trayOpen = new ToolStripMenuItem(); trayExit = new ToolStripMenuItem();
            trayOpen.Click += delegate { ShowForm(); };
            trayExit.Click += delegate { reallyExit = true; Close(); };
            tmenu.Items.Add(trayOpen); tmenu.Items.Add(trayExit);
            tray = new NotifyIcon { Icon = LoadAppIcon(), Visible = true, ContextMenuStrip = tmenu };
            tray.DoubleClick += delegate { ShowForm(); };

            ApplyLanguage();
        }

        // --- 言語 ---
        void SetLang(string lang)
        {
            Loc.Lang = lang;
            ApplyLanguage();
            CollectConfig();
        }

        bool IsRunning() { return monitor != null && monitor.Running; }

        void ApplyLanguage()
        {
            Text = Loc.T("title");
            tray.Text = Loc.T("a_title");
            miFile.Text = Loc.T("m_file"); miExit.Text = Loc.T("m_exit"); miClearCredentials.Text = Loc.T("m_clear_creds");
            miLang.Text = Loc.T("m_lang"); miJa.Checked = Loc.Lang == "ja"; miEn.Checked = Loc.Lang == "en";
            miHelp.Text = Loc.T("m_help"); miUsage.Text = Loc.T("m_usage"); miAbout.Text = Loc.T("m_about");
            trayOpen.Text = Loc.T("t_open"); trayExit.Text = Loc.T("t_exit");
            miOpenItem.Text = Loc.T("open_item");
            miToggleVisibility.Text = Loc.T("vis_toggle");
            box.Text = Loc.T("grp");
            lblSlug.Text = Loc.T("slug"); lblPlat.Text = Loc.T("plat"); lblInt.Text = Loc.T("intv");
            btnStart.Text = Loc.T(IsRunning() ? "stop" : "start");
            btnNow.Text = Loc.T("now");
            btnAllPublic.Text = Loc.T("vis_all_public"); btnAllPrivate.Text = Loc.T("vis_all_private");
            btnSetStatus.Text = Loc.T("set_status");
            btnMarket.Text = Loc.T("market_home");
            int selectedStatus = cmbStatus.SelectedIndex;
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new object[] { Loc.T("status_ingame"), Loc.T("status_online"), Loc.T("status_invisible") });
            cmbStatus.SelectedIndex = selectedStatus < 0 ? 0 : selectedStatus;
            list.Columns[0].Text = Loc.T("c_item"); list.Columns[1].Text = Loc.T("c_rank"); list.Columns[2].Text = Loc.T("c_mine");
            list.Columns[3].Text = Loc.T("c_low"); list.Columns[4].Text = Loc.T("c_state");
            list.Columns[5].Text = Loc.T("c_visibility"); list.Columns[6].Text = Loc.T("c_total_qty");
            if (lastRows != null) RenderRows(lastRows);
            UpdateStatus();
        }

        void UpdateStatus()
        {
            if (!IsRunning()) lblStatus.Text = Loc.T("st_idle");
            else if (lastCheck.HasValue) lblStatus.Text = Loc.F("st_last", lastCheck.Value.ToString("HH:mm:ss"));
            else lblStatus.Text = Loc.T("st_run");
        }

        static string StateText(RowState s)
        {
            return Loc.T(s == RowState.Under ? "s_under" : (s == RowState.Lowest ? "s_low" : "s_none"));
        }

        void OnListMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            ListViewItem hit = list.GetItemAt(e.X, e.Y);
            list.SelectedItems.Clear();
            if (hit == null) return;
            hit.Selected = true;
            hit.Focused = true;
        }

        void OpenSelectedItemPage()
        {
            if (list.SelectedItems.Count == 0) return;
            Row r = list.SelectedItems[0].Tag as Row;
            if (r == null || string.IsNullOrEmpty(r.Item)) return;
            string url = "https://warframe.market/items/" + Uri.EscapeDataString(r.Item);
            try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        void OpenMarketHome()
        {
            try { Process.Start(new ProcessStartInfo("https://warframe.market/") { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show(this, Loc.F("market_open_fail", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        static Bitmap CreateMarketIcon()
        {
            var bitmap = new Bitmap(40, 40);
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Brush dark = new SolidBrush(Color.FromArgb(25, 91, 99)))
            using (Brush teal = new SolidBrush(Color.FromArgb(89, 220, 199)))
            using (Brush light = new SolidBrush(Color.FromArgb(176, 247, 232)))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(dark, 1, 1, 38, 38);
                g.FillPolygon(light, new Point[] { new Point(20, 4), new Point(26, 14), new Point(20, 19), new Point(14, 14) });
                g.FillPolygon(teal, new Point[] { new Point(4, 12), new Point(15, 15), new Point(19, 20), new Point(13, 23) });
                g.FillPolygon(teal, new Point[] { new Point(36, 12), new Point(25, 15), new Point(21, 20), new Point(27, 23) });
                g.FillPolygon(light, new Point[] { new Point(8, 26), new Point(17, 22), new Point(20, 24), new Point(17, 30) });
                g.FillPolygon(light, new Point[] { new Point(32, 26), new Point(23, 22), new Point(20, 24), new Point(23, 30) });
                g.FillPolygon(teal, new Point[] { new Point(20, 36), new Point(15, 29), new Point(20, 24), new Point(25, 29) });
            }
            return bitmap;
        }

        static Icon LoadAppIcon()
        {
            try { return System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application; }
            catch (Exception) { return SystemIcons.Application; }
        }

        void ShowForm() { Show(); WindowState = FormWindowState.Normal; Activate(); }

        // --- 設定の保存/読込 ---
        static Config LoadConfig()
        {
            try { return new JavaScriptSerializer().Deserialize<Config>(File.ReadAllText(ConfigPath, Encoding.UTF8)) ?? new Config(); }
            catch (Exception) { return new Config(); }
        }

        void ClearSavedCredentials()
        {
            jwt = null;
            savedEmail = null;
            savedPassword = null;
            protectedCredentials = null;
            CollectConfig();
        }

        Config CollectConfig()
        {
            var c = new Config();
            c.Slug = txtSlug.Text.Trim();
            c.ProtectedCredentials = protectedCredentials;
            c.Interval = (int)numInt.Value;
            c.Platform = cmbPlat.SelectedItem as string ?? "pc";
            c.Lang = Loc.Lang;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
                File.WriteAllText(ConfigPath, new JavaScriptSerializer().Serialize(c), Encoding.UTF8);
            }
            catch (Exception) { }
            return c;
        }

        bool GetLoginInfo(ref string token, ref string email, ref string password, ref bool remember, ref bool fromSaved)
        {
            if (!string.IsNullOrEmpty(token)) return true;
            if (!string.IsNullOrEmpty(savedEmail) && !string.IsNullOrEmpty(savedPassword))
            {
                email = savedEmail;
                password = savedPassword;
                fromSaved = true;
                return true;
            }
            using (var login = new LoginDialog())
            {
                if (login.ShowDialog(this) != DialogResult.OK) return false;
                email = login.Email;
                password = login.Password;
                remember = login.RememberCredentials;
            }
            return true;
        }

        void SaveRememberedLogin(string email, string password, bool remember)
        {
            if (!remember || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return;
            string encrypted = CredentialProtection.Protect(email, password);
            UI(() => { savedEmail = email; savedPassword = password; protectedCredentials = encrypted; CollectConfig(); });
        }

        // --- 操作 ---
        void OnToggle(object sender, EventArgs e)
        {
            if (IsRunning())
            {
                monitor.Stop(); monitor = null;
                btnStart.Text = Loc.T("start");
                UpdateStatus();
                AddLog(Loc.T("l_stop"));
                return;
            }
            Config c = CollectConfig();
            if (c.Slug.Length == 0) { MessageBox.Show(this, Loc.T("m_slug_req"), Text); return; }
            string token = jwt, email = null, password = null;
            bool remember = false, fromSaved = false;
            if (!GetLoginInfo(ref token, ref email, ref password, ref remember, ref fromSaved)) return;
            btnStart.Enabled = false;
            btnNow.Enabled = false;
            lastCheck = null;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    if (token == null) token = Auth.SignIn(email, password, c.Platform);
                    string sessionToken = token;
                    UI(() =>
                    {
                        jwt = sessionToken;
                        monitor = new Monitor(c, sessionToken,
                            msg => UI(() => AddLog(msg)),
                            rows => UI(() => ShowRows(rows)));
                        monitor.Start();
                        btnStart.Text = Loc.T("stop");
                        btnStart.Enabled = true;
                        btnNow.Enabled = true;
                        UpdateStatus();
                        AddLog(Loc.F("l_start", c.Interval));
                    });
                    SaveRememberedLogin(email, password, remember);
                }
                catch (Exception ex)
                {
                    string msg = Loc.F("l_err", Auth.Describe(ex));
                    UI(() =>
                    {
                        if (fromSaved && Auth.StatusCode(ex) == 401) ClearSavedCredentials();
                        btnStart.Enabled = true;
                        btnNow.Enabled = true;
                        AddLog(msg);
                        MessageBox.Show(this, msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
            });
        }

        void OnSetStatus(object sender, EventArgs e)
        {
            int selected = cmbStatus.SelectedIndex;
            if (selected < 0) return;
            string[] statusValues = new string[] { "ingame", "online", "invisible" };
            string[] statusKeys = new string[] { "status_ingame", "status_online", "status_invisible" };
            string status = statusValues[selected];
            string displayStatus = Loc.T(statusKeys[selected]);

            string email = null, password = null;
            bool rememberCredentials = false;
            bool usingSavedCredentials = false;
            string currentJwt = jwt;
            if (currentJwt == null)
            {
                if (!string.IsNullOrEmpty(savedEmail) && !string.IsNullOrEmpty(savedPassword))
                {
                    email = savedEmail; password = savedPassword; usingSavedCredentials = true;
                }
                else
                {
                    using (var lg = new LoginDialog())
                    {
                        if (lg.ShowDialog(this) != DialogResult.OK) return;
                        email = lg.Email; password = lg.Password; rememberCredentials = lg.RememberCredentials;
                    }
                }
            }

            string platform = cmbPlat.SelectedItem as string ?? "pc";
            btnSetStatus.Enabled = false;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    string token = currentJwt;
                    if (token == null)
                    {
                        token = Auth.SignIn(email, password, platform);
                        string signedInToken = token;
                        UI(() => { jwt = signedInToken; if (monitor != null) monitor.SetToken(signedInToken); });
                        if (rememberCredentials)
                        {
                            string encrypted = CredentialProtection.Protect(email, password);
                            UI(() => { savedEmail = email; savedPassword = password; protectedCredentials = encrypted; CollectConfig(); });
                        }
                    }
                    Auth.SetUserStatus(token, status);
                    UI(() => AddLog(status == "invisible" ? Loc.T("status_invisible_done") : Loc.F("status_done", displayStatus)));
                }
                catch (Exception ex)
                {
                    string detail = Auth.Describe(ex);
                    UI(() =>
                    {
                        if (usingSavedCredentials && Auth.StatusCode(ex) == 401) ClearSavedCredentials();
                        string msg = Loc.F("status_fail", detail);
                        AddLog(msg);
                        MessageBox.Show(this, msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
                finally { UI(() => btnSetStatus.Enabled = true); }
            });
        }

        void OnToggleSelectedVisibility()
        {
            if (list.SelectedItems.Count == 0 || visibilityUpdateRunning) return;
            Row row = list.SelectedItems[0].Tag as Row;
            if (row == null || !row.Visible.HasValue) return;
            ChangeVisibility(row, !row.Visible.Value, false);
        }

        void OnSetAllVisibility(bool visible)
        {
            if (visibilityUpdateRunning || priceUpdateRunning) return;
            string target = Loc.T(visible ? "vis_target_public" : "vis_target_private");
            if (MessageBox.Show(this, Loc.F("vis_confirm", target), Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            ChangeVisibility(null, visible, true);
        }

        void ChangeVisibility(Row row, bool visible, bool all)
        {
            if (visibilityUpdateRunning || priceUpdateRunning) return;
            string token = jwt, email = null, password = null;
            bool remember = false, fromSaved = false;
            if (!GetLoginInfo(ref token, ref email, ref password, ref remember, ref fromSaved)) return;
            string platform = cmbPlat.SelectedItem as string ?? "pc";
            visibilityUpdateRunning = true;
            btnAllPublic.Enabled = btnAllPrivate.Enabled = false;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    if (token == null) token = Auth.SignIn(email, password, platform);
                    string sessionToken = token;
                    if (remember) SaveRememberedLogin(email, password, true);
                    if (all) Auth.UpdateAllSellVisibility(sessionToken, visible, platform);
                    else Auth.UpdateVisibility(sessionToken, row.OrderId, visible, platform);
                    UI(() =>
                    {
                        jwt = sessionToken;
                        if (monitor != null) monitor.SetToken(sessionToken);
                        if (lastRows != null)
                        {
                            foreach (Row item in lastRows)
                                if (all || item.OrderId == row.OrderId) item.Visible = visible;
                            RenderRows(lastRows);
                        }
                        string target = Loc.T(visible ? "vis_target_public" : "vis_target_private");
                        AddLog(Loc.F("vis_done", target));
                    });
                }
                catch (Exception ex)
                {
                    string detail = Auth.Describe(ex);
                    UI(() =>
                    {
                        if (fromSaved && Auth.StatusCode(ex) == 401) ClearSavedCredentials();
                        string msg = Loc.F("vis_fail", detail);
                        AddLog(msg);
                        MessageBox.Show(this, msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
                finally
                {
                    UI(() => { visibilityUpdateRunning = false; btnAllPublic.Enabled = btnAllPrivate.Enabled = true; });
                }
            });
        }
        // --- 価格変更 ---
        void OnChangePrice(object sender, EventArgs e)
        {
            if (priceUpdateRunning || visibilityUpdateRunning) return;
            if (list.SelectedItems.Count == 0) { MessageBox.Show(this, Loc.T("p_select"), Text); return; }
            Row r = (Row)list.SelectedItems[0].Tag;
            if (string.IsNullOrEmpty(r.OrderId)) { MessageBox.Show(this, Loc.T("p_noid"), Text); return; }

            int suggested = (r.LowValue.HasValue && r.LowValue.Value < r.MineValue) ? r.LowValue.Value : r.MineValue;
            int newPrice;
            using (var dlg = new PriceDialog(r, suggested))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                newPrice = dlg.Price;
            }

            string email = null, password = null;
            bool rememberCredentials = false;
            bool usingSavedCredentials = false;
            if (jwt == null)
            {
                if (!string.IsNullOrEmpty(savedEmail) && !string.IsNullOrEmpty(savedPassword))
                {
                    email = savedEmail; password = savedPassword; usingSavedCredentials = true;
                }
                else
                {
                    using (var lg = new LoginDialog())
                    {
                        if (lg.ShowDialog(this) != DialogResult.OK) return;
                        email = lg.Email; password = lg.Password; rememberCredentials = lg.RememberCredentials;
                    }
                }
            }

            string platform = cmbPlat.SelectedItem as string ?? "pc";
            string currentJwt = jwt;
            int oldPrice = r.MineValue;
            priceUpdateRunning = true;
            ThreadPool.QueueUserWorkItem(delegate
            {
                bool updating = false;
                try
                {
                    string token = currentJwt;
                    if (token == null)
                    {
                        token = Auth.SignIn(email, password, platform);
                        UI(() => { jwt = token; if (monitor != null) monitor.SetToken(token); });
                        if (rememberCredentials)
                        {
                            string encrypted = CredentialProtection.Protect(email, password);
                            UI(() => { savedEmail = email; savedPassword = password; protectedCredentials = encrypted; CollectConfig(); });
                        }
                    }
                    updating = true;
                    Auth.UpdateOrder(token, r, newPrice, platform);
                    UI(() =>
                    {
                        priceUpdateRunning = false;
                        r.MineValue = newPrice;
                        AddLog(Loc.F("p_done", oldPrice, newPrice));
                        if (IsRunning()) monitor.CheckNow();
                        else if (lastRows != null) RenderRows(lastRows);
                    });
                }
                catch (Exception ex)
                {
                    int code = Auth.StatusCode(ex);
                    string detail = Auth.Describe(ex);
                    bool expired = updating && code == 401;
                    UI(() =>
                    {
                        priceUpdateRunning = false;
                        if (!updating && usingSavedCredentials) ClearSavedCredentials();
                        string msg;
                        if (expired) { jwt = null; msg = Loc.F("p_fail", detail); }
                        else msg = Loc.F(updating ? "p_fail" : "lg_fail", detail);
                        AddLog(msg);
                        MessageBox.Show(this, msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
            });
        }

        // --- 画面更新（UIスレッドで実行） ---
        void UI(Action a)
        {
            if (IsDisposed) return;
            try { BeginInvoke(a); } catch (InvalidOperationException) { } // ObjectDisposedExceptionも含む
        }

        void AddLog(string text)
        {
            txtLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + text + "\r\n");
        }

        void ShowRows(List<Row> rows)
        {
            lastRows = rows;
            lastCheck = DateTime.Now;
            RenderRows(rows);
            UpdateStatus();
        }

        void RenderRows(List<Row> rows)
        {
            string sel = null;
            if (list.SelectedItems.Count > 0)
            {
                Row sr = list.SelectedItems[0].Tag as Row;
                if (sr != null) sel = sr.Key;
            }
            list.BeginUpdate();
            list.Items.Clear();
            foreach (Row r in rows)
            {
                var it = new ListViewItem(new string[] {
                    r.Item, r.RankText, r.MineValue.ToString(),
                    r.LowValue.HasValue ? r.LowValue.Value.ToString() : "-", StateText(r.State),
                    Loc.T(r.Visible.HasValue ? (r.Visible.Value ? "visibility_public" : "visibility_private") : "visibility_unknown"),
                    r.SameItemQuantity.ToString() });
                it.Tag = r;
                if (r.State == RowState.Under) it.BackColor = Color.FromArgb(255, 214, 214);
                list.Items.Add(it);
                if (sel != null && r.Key == sel) it.Selected = true;
            }
            list.EndUpdate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 監視中に×を押したときは、終了せずタスクトレイに隠す
            if (!reallyExit && e.CloseReason == CloseReason.UserClosing && IsRunning())
            {
                e.Cancel = true;
                Hide();
                return;
            }
            CollectConfig();
            if (monitor != null) monitor.Stop();
            tray.Visible = false;
            tray.Dispose();
            base.OnFormClosing(e);
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; // TLS 1.2
            ServicePointManager.Expect100Continue = false;
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}
