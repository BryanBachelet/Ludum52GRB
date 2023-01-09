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

        public Sprite[] seedSelectorImage;
        public Sprite[] ThrowVegetableImage;
        public Sprite compostImage;

        public Image[] playerInterfaceImage;
        private GameObject lastVegetableSelected;

        public GameObject notificationPrefab;
        public GameObject notificationPrefabRemove;
        public GameObject notificationHolderPosition;
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
                playerInterfaceImage[1].sprite = seedSelectorImage[m_indexSeedSelected];
            }
        }

    

        public void ChangeVegetableInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                ChangeObjectSelected((int)Mathf.Sign(ctx.ReadValue<float>()), ref m_indexVegetableSelected, m_vegetable.Length);
                playerInterfaceImage[2].sprite = ThrowVegetableImage[m_indexVegetableSelected];
            }
        }

        #endregion

        private void ThrowVegetable()
        {
            if (m_vegetableCarryNumber[m_indexVegetableSelected] <= 0)
            {
                return;
            }

            GameObject go = GameObject.Instantiate(m_throwSeed, transform.position - new Vector3(0,2,0), transform.rotation);
            GlobalSoundManager.PlayOneShot(1, Vector3.zero);
            ThrowVegetable throwVegetable = go.GetComponent<ThrowVegetable>();
            GenerateNotification(notificationPrefabRemove, ThrowVegetableImage[m_indexVegetableSelected], "- 1");
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
            GlobalSoundManager.PlayOneShot(6, Vector3.zero);
            GameObject go = GameObject.Instantiate(m_vegetable[m_indexSeedSelected], cell.position, transform.rotation);
            GenerateNotification(notificationPrefabRemove, seedSelectorImage[m_indexSeedSelected], "- 1");
            Vegetable vege = go.GetComponent<Vegetable>();
            vege.InitVegetable(transform, cell);
            m_seeds[m_indexSeedSelected]--;
            UpdateUIFeedback();
        }

        private void Harvest(Cell cell)
        {
            if (cell.currentVegetable == null || cell.currentVegetable.GetState() != Vegetable.State.Harvest) return;

            GlobalSoundManager.PlayOneShot(4, Vector3.zero);
            GlobalSoundManager.PlayOneShot(7, Vector3.zero);
            int index = (int)cell.currentVegetable.m_type;
            GenerateNotification(notificationPrefab, ThrowVegetableImage[index], "+ 1");
            cell.currentVegetable.GetCollect();
            cell.currentVegetable = null;
            cell.isEmpty = true;

            m_vegetableCarryNumber[index]++;
            UpdateUIFeedback();
        }

        private void CheckHarvestPosssibility(Cell cell)
        {
            if (cell.currentVegetable == lastVegetableSelected) return;
            else
            {
                if (lastVegetableSelected)
                {
                    SetLayerRecursively(lastVegetableSelected, 0);
                }
            }
            if (cell.currentVegetable == null || cell.currentVegetable.GetState() != Vegetable.State.Harvest) return;

            lastVegetableSelected = cell.currentVegetable.gameObject;
            SetLayerRecursively(lastVegetableSelected, 6);


        }
        private void Update()
        {
            CellPreVisual();
            Cell cell = m_gridManager.ClosestCells(transform.position + transform.forward * m_interactionDistance);
            CheckHarvestPosssibility(cell);
        }

        private void SetLayerRecursively(GameObject obj, int layerApplied)
        {
            if (null == obj)
            {
                return;
            }

            obj.layer = layerApplied;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, layerApplied);
            }
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
            GlobalSoundManager.PlayOneShot(3, Vector3.zero);
            GenerateNotification(notificationPrefab, compostImage, "+ 1");
            m_rottenVegetableCarry++;
            UpdateUIFeedback();
        }

        private void GiveComposter(Collider other)
        {
            if (other.tag != "Composter") return;

            other.GetComponent<ComposterBehavior>().AddVegetable(m_rottenVegetableCarry);
            GlobalSoundManager.PlayOneShot(0, other.transform.position);
            GenerateNotification(notificationPrefabRemove, compostImage, "- " + m_rottenVegetableCarry);
            m_rottenVegetableCarry = 0;
            UpdateUIFeedback();
        }

        private void TakeGroupSeed(Collider other)
        {
            if (other.tag != "Seed") return;

            GlobalSoundManager.PlayOneShot(5, Vector3.zero);
            GroupSeed seed = other.GetComponent<GroupSeed>();
            int index = (int)seed.allSeed[0].type;
            m_seeds[index] += seed.allSeed.Length;
            GenerateNotification(notificationPrefab, seedSelectorImage[index], "+ " + seed.allSeed.Length);
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

        private void GenerateNotification(GameObject PrefabUsed, Sprite spriteUsed, string textused)
        {
            GameObject notif = Instantiate(PrefabUsed, notificationHolderPosition.transform.position, notificationHolderPosition.transform.rotation, notificationHolderPosition.transform);
            notif.GetComponentInChildren<Text>().text = textused;
            notif.GetComponentInChildren<Image>().sprite = spriteUsed;
        }
    }
}
