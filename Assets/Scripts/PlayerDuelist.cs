using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDuelist : Duelist {
    // TODO: Cursor as its own script?
    [SerializeField] private SpriteRenderer cursor;
    [SerializeField] private float cursorMoveTime = 1f;

    protected override DuelistState StartState => new DrawState<PlayerBaseState>();

    protected override void InitForGame() {
        for (int i = 0; i < 4; i++) {
            DrawCard();
        }
    }

    public void ShowCursor() { cursor.enabled = true; }
    public void HideCursor() { cursor.enabled = false; }

    public Coroutine MoveCursorToCard(Card card, bool instant = false) {
        return StartCoroutine(MoveCursor(card.transform.position, instant));
    }

    IEnumerator MoveCursor(Vector3 dest, bool instant) {
        Vector3 start = cursor.transform.position;
        float t = 0;

        if (!instant) {
            while (t < cursorMoveTime) {
                cursor.transform.position = Vector3.Lerp(start, dest, t / cursorMoveTime);
                yield return null;
                t += Time.deltaTime;
            }
        }

        cursor.transform.position = dest;
    }
}
