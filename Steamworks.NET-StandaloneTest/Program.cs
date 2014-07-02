using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Steamworks;

namespace SteamworksNET_StandaloneTest {
	class Program {
		static CallResult<NumberOfCurrentPlayers_t> NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
		static CallResult<LeaderboardFindResult_t> m_callResultFindLeaderboard;
		static Callback<PersonaStateChange_t> m_PersonaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
		static Callback<UserStatsReceived_t> m_UserStatsReceived;

		static void Main(string[] args) {
			if(!SteamAPI.Init()) {
				Console.WriteLine("SteamAPI.Init() failed!");
				return;
			}
			
			if (!Packsize.Test()) {
				Console.WriteLine("You're using the wrong Steamworks.NET Assembly for this platform!");
				return;
			}

			m_callResultFindLeaderboard = CallResult<LeaderboardFindResult_t>.Create(OnFindLeaderboard);
			m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);

			Console.WriteLine("CurrentGameLanguage: " + SteamApps.GetCurrentGameLanguage());
			Console.WriteLine("PersonaName: " + SteamFriends.GetPersonaName());
			string folder;
			uint length = SteamApps.GetAppInstallDir(SteamUtils.GetAppID(), out folder, 260);
			Console.WriteLine("AppInstallDir: " + length + " " + folder);

			SteamUserStats.RequestCurrentStats();
			Console.WriteLine("Reqesting Current Stats");

			while (true) {
				// Must be called from the primary thread.
				SteamAPI.RunCallbacks();

				if (Console.KeyAvailable) {
					ConsoleKeyInfo info = Console.ReadKey(true);

					if (info.Key == ConsoleKey.Escape) {
						break;
					}
					else if (info.Key == ConsoleKey.Spacebar) {
						SteamUserStats.RequestCurrentStats();
						Console.WriteLine("Reqesting Current Stats");
					}
					else if (info.Key == ConsoleKey.D1) {
						SteamAPICall_t hSteamAPICall = SteamUserStats.FindLeaderboard("Quickest Win");
						m_callResultFindLeaderboard.Set(hSteamAPICall);
						Console.WriteLine("FindLeaderboard() - " + hSteamAPICall);
					}
					else if (info.Key == ConsoleKey.D2) {
						SteamAPICall_t hSteamAPICall = SteamUserStats.GetNumberOfCurrentPlayers();
						NumberOfCurrentPlayers.Set(hSteamAPICall);
						Console.WriteLine("GetNumberOfCurrentPlayers() - " + hSteamAPICall);
					}
				}

				Thread.Sleep(50);
			}

			SteamAPI.Shutdown();
		}

		static void OnFindLeaderboard(LeaderboardFindResult_t pCallback, bool bIOFailure) {
			Console.WriteLine("[" + LeaderboardFindResult_t.k_iCallback + " - LeaderboardFindResult] - " + pCallback.m_bLeaderboardFound + " -- " + pCallback.m_hSteamLeaderboard);
		}

		static void OnUserStatsReceived(UserStatsReceived_t pCallback) {
			Console.WriteLine("[" + UserStatsReceived_t.k_iCallback + " - UserStatsReceived] - " + pCallback.m_eResult + " -- " + pCallback.m_nGameID + " -- " + pCallback.m_steamIDUser);
		}

		static void OnPersonaStateChange(PersonaStateChange_t pCallback) {
			Console.WriteLine("[" + PersonaStateChange_t.k_iCallback + " - PersonaStateChange] - " + pCallback.m_ulSteamID + " -- " + pCallback.m_nChangeFlags);
		}

		static void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure) {
			Console.WriteLine("[" + NumberOfCurrentPlayers_t.k_iCallback + " - NumberOfCurrentPlayers] - " + pCallback.m_bSuccess + " -- " + pCallback.m_cPlayers);
		}
	}
}
