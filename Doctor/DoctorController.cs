using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoctorController : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2.0f; // Ž������ �̵� �ӵ�
    [SerializeField] private float chaseSpeed = 4.0f;// �߰� ���� �� �ӵ�
    [SerializeField] private Animator anim; // ���� ���ʹ� �ִϸ�����
    [SerializeField] private const float chaseRange = 5.0f; // �߰� ���� ����--> �� �Ÿ��� ����� �߰� ���� ����
    [SerializeField] private const float detectionRange = 3.0f; // Ž�� ���� ����--> �� �Ÿ� ���� �÷��̾ ������ �߰� ���·� ����
    [SerializeField] private const float attackRange = 1.0f;//���� ���� ����
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent agent;
    public JailDoorEventController Flag;//�÷��̾ �̺�Ʈ  Ʈ���Ÿ� ����ߴ��� ����
    private Define.DoctorState currentState;//���� ���� ����
    private float distanceToPlayer;// �÷��̾�� ���� �� �Ÿ�
    private bool isCoroutineRunning = false; //���� ���̸� ���� �ڷ�ƾ ���� ���� �÷���


     void Start() {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentState = Define.DoctorState.IDLE;
        agent.stoppingDistance = attackRange;// �׺�޽� ������Ʈ ��ü�� ���� �����Ÿ��� ������ ����
        agent.speed = patrolSpeed;//�׺�޽� ������Ʈ ��ü�� ���� �ӵ��� ������ ����
    }

    void Update() 
    {
        if(target!=null && currentState !=Define.DoctorState.ATTACK)// target�� �÷��̾ �Ҵ�Ǿ��� attack���°� �ƴ� ��� �÷��̾��� ��ġ�� �������� ����
        {
            agent.SetDestination(target.position);
        }    
    }
    void AwakeDoctor()// �÷��̾ �̺�Ʈ Ʈ���ſ� ���� �� ���� �ൿ ����
    {
         if(Flag.isEnter && target==null)//�÷��̾ �̺�Ʈ Ʈ���ſ� ���� + ���� target�� null�̸� �÷��̾ ã�� target�� �Ҵ�
         {
            target = GameObject.FindWithTag("PLAYER").transform;
            SetState(Define.DoctorState.WALKING);//�÷��̾������� �ɾ�´�. ���� ���� ���� �ƽ� �Ǵ� ���̾�α׸� �߰��� ��.
         }
    }


    private void SetState(Define.DoctorState newState)// ������ ���� ��ȭ ����Ī�� ���� �Լ�
    {
        if(currentState!=newState)
        {
            currentState = newState;//���� ���� �ߺ� ȣ���� �ƴ� ��� ���� ���� ����Ī
        }
    }
    private void PlayAnimation(string animationName)// ������ ���� �� �ִϸ��̼� ����� ���� �Լ�
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName(animationName))// ���� �ִϸ����Ϳ��� ����Ǵ� �ִϸ��̼� ���¸� üũ -> animationName�̶�� �̸��� �ִϸ��̼��� ����ǰ� ���� �ʴٸ� ���ǹ� ����
        {
            anim.Play(animationName, 0, 0);// ���� �� normalizedTime = 0 --> �ִϸ��̼��� ���� ���۵��� �ʾҴٴ� ��.
        }
    }

    private IEnumerator FSM()//���� ���¸� ������ ���� ���� ��� �ڷ�ƾ
    {
        while(true)
        {
            switch(currentState)//���� ���� ������ ���� �ݺ��ϸ� ���� ���¸� �����Ѵ�.
            {
                case Define.DoctorState.IDLE:
                yield return StartCoroutine(IDLE());break;
                
                case Define.DoctorState.WALKING:
                yield return StartCoroutine(WALKING());break;
                
                case Define.DoctorState.RUNNING:
                yield return StartCoroutine(RUNNING());break;
                
                case Define.DoctorState.ATTACK:
                yield return StartCoroutine(ATTACK());break; 
                 
            }yield return null;
        }
    }
    private IEnumerator IDLE()
    {
        PlayAnimation("IDLE");
        yield return new WaitForSeconds(1.0f);
        if(Flag.isEnter && target!=null)
        {
            SetState(Define.DoctorState.WALKING);//�÷��̾ Ʈ���ſ� ���� �� walking���·� ��ȯ
        }
    }
    private IEnumerator WALKING()
    {
        PlayAnimation("WALKING");
        agent.speed = patrolSpeed;
        if(target!=null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);// �÷��̾�� ���� �� �Ÿ� ���
            if(distanceToPlayer <=detectionRange)
            {
                SetState(Define.DoctorState.RUNNING);// �� ���� �Ÿ��� Ž�� �Ÿ� ���� ������ �߰� ���·� ��ȯ
            }
        }yield return new WaitForSeconds(0.1f);// 0.1�� ��ȯ ���
    }
    private IEnumerator RUNNING()
    {
        PlayAnimation("RUNNING");
        agent.speed  =  chaseSpeed;
        if(target!=null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if(distanceToPlayer <= attackRange)
            {
                SetState(Define.DoctorState.ATTACK);// �� ���� �Ÿ��� ���� ���� ���� �̳���� ���� ���·� ��ȯ
            }
            else if(distanceToPlayer > chaseRange)
            {
                SetState(Define.DoctorState.WALKING);//�� ���� �Ÿ��� �߰� ���� ������ �Ѿ�ٸ� walking ���·� ��ȯ
            }
        }yield return new WaitForSeconds(0.1f);// 0.1�� ��ȯ ���
    }
    private IEnumerator ATTACK()
    {
        PlayAnimation("ATTACK");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);//���� �ִϸ��̼� �Ϸ� ���
        if(target!=null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if(distanceToPlayer > attackRange)
            {
                SetState(Define.DoctorState.RUNNING);// ���� ���� ������ ����ٸ� �߰� ���·� ��ȯ
            }
        }
    }
}
