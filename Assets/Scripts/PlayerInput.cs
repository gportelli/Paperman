using UnityEngine;
using System.Collections;
using Rewired;

public class PlayerInput : IZSingleton<PlayerInput> {
    public Player Input;

	override public void Init () {
        IsPersistent = false;

        Input = ReInput.players.GetPlayer(0);
	}
}
