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
        [SerializeField] private GameObject m_throwSeed;
        [SerializeField] private GameObject[] m_vegetable = new GameObject[3];

        [Header("Feedbacks parameters")]
        [SerializeField] private GameObject m_quadFeedback;
        [SerializeField] private GameObject m_interactionFeedback;
        [SerializeField] private Text m_seedText;
        [SerializeField] private Text m_composterText;
        [SerializeField] private Text m_vegetableText;

        [SerializeField] private int m_maxSeeds = 15;


        private int[] m_seeds = new int[4];
        private int m_indexSeedSelected = 0;
        private int m_indexVegetableSelected = 0;
        private int[] m_vegetableCarryNumber = new int[4];
        private int m_rottenVegetableCarry;


        private void Start()
        {
            UpdateUIFeedback();
        }

        private void UpdateUIFeedback()
        {
            m_seedText.text = m_seeds[m_indexSeedSelected].ToString();
            m_composterText.text = m_rottenVegetableCarry.ToString();
            m_vegetableText.text = m_vegetableCarryNumber[m_indexVegetableSelected].ToString();
        }

        #region Input

        public void ThrowInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started) ThrowVegetable();
        }


        public void PlantInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started) HarvestInteraction();
        }

        public void ChangeSeedInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                ChangeObjectSelected((int)ctx.ReadValue<float>(),ref m_indexSeedSelected,m_seeds.Length);
            }
        }

    

        public void ChangeVegetableInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                ChangeObjectSelected((int)Mathf.Sign(ctx.ReadValue<float>()), ref m_indexVegetableSelected, m_vegetable.Length);
            }
        }

        #endregion

        private void ThrowVegetable()
        {
            if (m_vegetableCarryNumber[m_indexVegetableSelected] <= 0)
            {
                return;
            }

            GameObject go = GameObject.Instantiate(m_throwSeed, transform.position, transform.rotation);
            ThrowVegetable throwVegetable = go.GetComponent<ThrowVegetable>();
            throwVegetable.directon = transform.forward;
            m_vegetableCarryNumber[m_indexVegetableSelected]--;
            UpdateUIFeedback();
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

            GameObject go = GameObject.Instantiate(m_vegetable[m_indexSeedSelected], cell.position, transform.rotation);
            Vegetable vege = go.GetComponent<Vegetable>();
            vege.InitVegetable(transform, cell);
            m_seeds[m_indexSeedSelected]--;
            UpdateUIFeedback();
        }

        private void Harvest(Cell cell)
        {
            if (cell.currentVegetable == null || cell.currentVegetable.GetState() != Vegetable.State.Harvest) return;

            int index = (int)cell.currentVegetable.m_type;
            cell.currentVegetable.GetCollect();
            cell.currentVegetable = null;
            cell.isEmpty = true;

            m_vegetableCarryNumber[index]++;
            UpdateUIFeedback();
        }

        private void Update()
        {
            CellPreVisual();
        }

        private void CellPreVisual()
        {
            m_quadFeedback.SetActive(true);
            m_interactionFeedback.SetActive(false);

            Cell cellClosest = m_gridManager.ClosestCells(transform.position + transform.forward * m_interactionDistance);
            m_quadFeedback.transform.position = cellClosest.position + new Vector3(0, 0.1f, 0);
            m_interactionFeedback.transform.position = cellClosest.position + new Vector3(0, 1f, 0.5f);

            if (cellClosest.isEmpty) return;

            m_quadFeedback.SetActive(false);

            if (cellClosest.currentVegetable && cellClosest.currentVegetable.GetState() != Vegetable.State.Harvest) return;

            m_interactionFeedback.SetActive(true);

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
            UpdateUIFeedback();
        }

        private void GiveComposter(Collider other)
        {
            if (other.tag != "Composter") return;

            other.GetComponent<ComposterBehavior>().AddVegetable(m_rottenVegetableCarry);
            m_rottenVegetableCarry = 0;
            UpdateUIFeedback();
        }

        private void TakeGroupSeed(Collider other)
        {
            if (other.tag != "Seed") return;

            GroupSeed seed = other.GetComponent<GroupSeed>();
            int index = (int)seed.allSeed[0].type;
            m_seeds[index] += seed.allSeed.Length;
            m_seeds[index] = Mathf.Clamp(m_seeds[index], 0, m_maxSeeds);
            Destroy(other.gameObject);

            if (m_indexSeedSelected != index) return;

            UpdateUIFeedback();

        }

        private void ChangeObjectSelected(int input, ref int index, int arrayLength)
        {
            index += input;

            if (index == arrayLength) index = 0;
            if (index ==  -1 ) index = arrayLength - 1;

            UpdateUIFeedback();
        }
    }
}
