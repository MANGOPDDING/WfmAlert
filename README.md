# WfmAlert

An unofficial Windows desktop tool for monitoring and managing your Warframe Market sell orders.

## Features

- Compare prices against other sellers who are **In Game**.
- Show public/private visibility and the total quantity listed for the same item.
- Change an order's price by double-clicking its row.
- Toggle visibility for one order from the row's right-click menu, or make all sell orders public/private.
- Run periodic checks or a one-time check, and view alerts in the in-app log.
- Open the Warframe Market item page or home page from the app.

## Build

On Windows with .NET Framework 4.x, put `build.bat`, `WfmAlert_market_enhancements.cs`, and `WfmAlert.ico` together and run `build.bat`. It creates `ReleaseAsset/WfmAlert.exe`.

## Sign-in and privacy

The source code contains no hard-coded account email, password, API token, Windows user path, or account slug. The slug example in the help text is generic. The app asks each user for their own Warframe Market credentials when needed. If “Save login” is selected, the app protects credentials with Windows Data Protection for that Windows user and stores the protected value in the user's AppData configuration file. Credentials and tokens are not included in this repository.

This tool is unofficial and is not affiliated with Digital Extremes or Warframe Market. Its sign-in currently uses Warframe Market's legacy v1 flow, which is deprecated and unsupported; it may stop working if the service changes. Review [Warframe Market's API rules](https://docs.warframe.market/docs/rules/overview/) before using or distributing the tool. API and automation policies may change.

## License

No license is included yet. Public visibility on GitHub does not by itself grant permission to reuse, modify, or redistribute this code. Add a license if you want to grant those rights.
