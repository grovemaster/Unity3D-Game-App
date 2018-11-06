# Unity3D-Game-App
Fan implementation of Hunter x Hunter game Gungi.  Created using Unity3D and SveltoECS.
Rules implemented according to: https://mmmmalo.tumblr.com/post/74510568781/rules-of-gungi
Some exceptions to the rules, explained in rules screen.
Game is not polished, but is in working state (for me).  Feel free to download and expland!

Takeaway of using Svelto ECS for creating a turn-based game is
* Turn-based games don't do polling
* Loading a save game file can be done with a single entity engine
* Complex rules can lead to some pretty gnarly step logic
