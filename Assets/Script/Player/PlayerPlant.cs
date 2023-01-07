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
        [SerializeField] private GameObject m_throwSeed;
        [SerializeField] private GameObject quadFeedback;

        private int m_vegetableCarryNumber;

        public void ThrowInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started) ThrowVegetable();
        }

        private void ThrowVegetable()
        {
            if (m_vegetableCarryNumber <= 0)
            {
                Debug.LogError("No Vegetable carry");
                return;
            }

            GameObject go = GameObject.Instantiate(m_throwSeed, transform.position, transform.rotation);
            ThrowVegetable throwVegetable = go.GetComponent<ThrowVegetable>();
            throwVegetable.directon = transform.forward;
            m_vegetableCarryNumber--;
        }

        public void PlantInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started) HarvestInteraction();
        }

        private void HarvestInteraction()
        {
            Cell cell = m_gridManager.ClosestCells(transform.position + transform.forward * m_interactionDistance);
            Plant(cell);
            Harvest(cell);
        }

        private void Plant(Cell cell)
        {
            if (!cell.isEmpty) return;

            cell.isEmpty = false;
            GameObject go = GameObject.Instantiate(m_seed, cell.position, transform.rotation);
            Vegetable vege = go.GetComponent<Vegetable>();
            vege.InitVegetable(transform, cell);
        }

        private void Harvest(Cell cell)
        {
            if (cell.currentVegetable == null || cell.currentVegetable.GetState() != Vegetable.State.Harvest) return;

            cell.currentVegetable.GetCollect();
            cell.currentVegetable = null;
            cell.isEmpty = true;
            m_vegetableCarryNumber++;
        }

        private void Update()
        {
           Cell cellClosest = m_gridManager.ClosestCells(transform.position + transform.forward * m_interactionDistance);
           quadFeedback.transform.position = cellClosest.position + new Vector3(0,0.1f,0);
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
