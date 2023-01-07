using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;


namespace Player
{

    public class PlayerPlant : MonoBehaviour
    {
        [SerializeField] private GridManager m_gridManager;
        [SerializeField] private float m_interactionDistance;
        [SerializeField] private GameObject m_seed;

        public void PlantInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started) Plant();
        }

        private void Plant()
        {
            Cell cell = m_gridManager.ClosestCells(transform.position + transform.forward * m_interactionDistance);
            if (cell.isEmpty)
            {
                cell.isEmpty = false;
                GameObject go = GameObject.Instantiate(m_seed, cell.position, transform.rotation);
                Vegetable vege = go.GetComponent<Vegetable>();
                vege.Init(transform, cell);

            }
        }

        private void OnDrawGizmos()
        {
            if (EditorApplication.isPlaying)
            {
                m_gridManager.DrawClosestCellPosition(transform.position + transform.forward * m_interactionDistance);
            }
        }
    }
}
