mergeInto(LibraryManager.library, {
  Hello: function () {
    window.alert("Hello, world!");
  },

  Initialization: function (url, key) {
    if (window.unityInstance && window.Telegram.WebApp) {
      const u = UTF8ToString(url);
      const k = UTF8ToString(key);
      const _supabase = supabase.createClient(u, k);
      window._supabase = _supabase;
      console.log("Supabase Instance: ", _supabase);
    }
  },

  SendScore: async function (score, game) {
    if (window.unityInstance && _supabase && window.Telegram.WebApp) {
      try {
        await window._supabase.from("scores").insert({
          score: score,
          game: game,
          telegramid: window.Telegram.WebApp.initDataUnsafe.user.id,
          username: window.Telegram.WebApp.initDataUnsafe.user.username,
          firstname: window.Telegram.WebApp.initDataUnsafe.user.first_name,
        });
      } catch (SendScoreError) {
        console.log({ SendScoreError });
      }
    }
  },

  GetLeaderboard: async function (leaderboard) {
    try {
      if (window.unityInstance && _supabase && window.Telegram.WebApp) {
        const view = UTF8ToString(leaderboard);
        console.log({ view });
        const { data, error } = await window._supabase.from(view).select("*");
        console.log({ data });
        const arr = data.map(item => {
          return {username:item.username, score:Number(item.score)};
        });
        console.log({leaderboard:arr});
        window.unityInstance.SendMessage(
            "GameManager",
            "OnLeaderboardReceived",
           JSON.stringify(arr)
        );
      }
      // );
    } catch (getLeaderboardError) {
      console.log({ getLeaderboardError });
    }
  },
});
