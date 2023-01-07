using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMouvement : MonoBehaviour
    {
        [SerializeField] private float m_speed;

        private PlayerInput m_playerInput;
        private Rigidbody m_rigidbody;

        private Vector3 m_directionInput;


        private void Start()
        {
            InitComponent();
        }

        private void InitComponent()
        {
            m_playerInput = GetComponent<PlayerInput>();
            m_rigidbody = GetComponent<Rigidbody>();
        }

        public void MoveInput(InputAction.CallbackContext ctx)
        {

            if (ctx.performed)
            {
                Vector2 input = ctx.ReadValue<Vector2>();
                m_directionInput = new Vector3(input.x, 0, input.y);
            }
            if (ctx.canceled)
            {
                m_directionInput = Vector3.zero;

            }
        }


        private void Update()
        {
            Orientation();
            Move();
        }

        private void Move()
        {
            m_rigidbody.velocity = m_directionInput.normalized * m_speed;
        }

        private void Orientation()
        {
            if (m_directionInput != Vector3.zero)
            {
                float angle = Vector3.SignedAngle(Vector3.forward, m_directionInput, Vector3.up);
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }
    }
}
