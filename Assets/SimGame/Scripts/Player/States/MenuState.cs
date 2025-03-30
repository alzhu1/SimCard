using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class MenuState : SimPlayerState {
        private Menu menu;

        protected override void Enter() {
            base.Enter();

            menu = new Menu(player.SimGameManager.EventBus.OnDisplayInteractOptions);
            menu.OpenMenu();
        }

        protected override void Exit() {
            base.Exit();

            menu.CloseMenu();
        }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    player.SimGameManager.PlayBackActionSound();
                    nextState = new RegularState();
                    break;
                }

                // Menu inputs
                if (Input.GetKeyDown(KeyCode.Space)) {
                    string option = menu.SelectedOption;

                    switch (option) {
                        case Menu.MenuOption.Deck: {
                            player.SimGameManager.PlayAdvanceSound();
                            player.SimGameManager.EventBus.OnInteractionEvent.Raise(new("StartDeckBuild", null, 0));
                            break;
                        }

                        case Menu.MenuOption.Exit: {
                            player.SimGameManager.PlayBackActionSound();
                            nextState = new RegularState();
                            break;
                        }

                        default: {
                            Debug.LogWarning($"Unexpected option: {option}");
                            break;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    player.SimGameManager.PlayOptionMoveSound();
                    menu.UpdateOptionIndex(-1);
                }

                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    player.SimGameManager.PlayOptionMoveSound();
                    menu.UpdateOptionIndex(1);
                }

                yield return null;
            }
        }
    }
}
