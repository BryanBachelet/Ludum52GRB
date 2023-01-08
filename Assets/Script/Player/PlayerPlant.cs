using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.InputSystem;


namespace Player
{

    public class PlayerPlant : MonoBehaviour
    {
        [SerializeField] private GridManager m_gridManager;
        [SerializeField] private float m_interactionDistance;
        [SerializeField] private GameObject m_vegetable;
        [SerializeField] private GameObject m_throwSeed;
        [SerializeField] private GameObject m_quadFeedback;
        [SerializeField] private GameObject m_imagefeedback;
        [SerializeField] private int m_maxSeeds = 15;


        [SerializeField]  private int[] m_seeds = new int[1];
        private int m_vegetableCarryNumber;
        private int m_rottenVegetableCarry;

        #region Input

        public void ThrowInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started) ThrowVegetable();
        }


        public void PlantInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started) HarvestInteraction();
        }
        #endregion

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

        private void HarvestInteraction()
        {
            Cell cell = m_gridManager.ClosestCells(transform.position + transform.forward * m_interactionDistance);
            Plant(cell);
            Harvest(cell);
        }

        private void Plant(Cell cell)
        {
            if (!cell.isEmpty || m_seeds[0] == 0) return;

            cell.isEmpty = false;
           
            GameObject go = GameObject.Instantiate(m_vegetable, cell.position, transform.rotation);
            Vegetable vege = go.GetComponent<Vegetable>();
            vege.InitVegetable(transform, cell);
            m_seeds[0]--;
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
            CellPreVisual();
        }

        private void OnDrawGizmos()
        {
            if (EditorApplication.isPlaying)
            {
                m_gridManager.DrawClosestCellPosition(transform.position + transform.forward * m_interactionDistance);
            }
        }

        private void CellPreVisual()
        {
            m_quadFeedback.SetActive(true);
            m_imagefeedback.SetActive(false);

            Cell cellClosest = m_gridManager.ClosestCells(transform.position + transform.forward * m_interactionDistance);
            m_quadFeedback.transform.position = cellClosest.position + new Vector3(0, 0.1f, 0);
            m_imagefeedback.transform.position = cellClosest.position + new Vector3(0, 1f, 0.5f);

            if (cellClosest.isEmpty) return;

            m_quadFeedback.SetActive(false);

            if (cellClosest.currentVegetable && cellClosest.currentVegetable.GetState() != Vegetable.State.Harvest) return;

            m_imagefeedback.SetActive(true);

        }

        private void OnTriggerEnter(Collider other)
        {
            CollectVegetable(other);
            GiveComposter(other);
            TakeGroupSeed(other);
        }

        private void CollectVegetable(Collider other)
        {
            if (other.tag != "Collectable") return;

            other.GetComponent<Vegetable>().GetCollect();
            m_rottenVegetableCarry++;
        }

        private void GiveComposter(Collider other)
        {
            if (other.tag != "Composter") return;

            other.GetComponent<ComposterBehavior>().AddVegetable(m_rottenVegetableCarry);
        }

        private void TakeGroupSeed(Collider other)
        {
            if (other.tag != "Seed") return;

            GroupSeed seed = other.GetComponent<GroupSeed>();
            int index = (int)seed.allSeed[0].type;
            m_seeds[index] += seed.allSeed.Length;
            m_seeds[index] = Mathf.Clamp(m_seeds[index], 0, m_maxSeeds);
            Destroy(other.gameObject);
        }
    }
}
