using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    public HealthBar barPrefab;
    public SpriteRenderer spriteRenderer;
    
    public int baseDamage = 1;
    public int baseHealth;
    [Range(1, 5)]
    public int range = 1;
    public float attackSpeed = 1f;
    public float movementSpeed = 1f;

    public Team myTeam;
    protected Node currentNode;
    protected BaseEntity currentTarget = null;
    
    public Node CurrentNode => currentNode;
    
    protected bool HasEnemy => currentTarget != null;
    protected bool IsInRange => currentTarget != null && Vector3.Distance(this.transform.position, currentTarget.transform.position) <= range;
    protected bool moving;
    protected Node destination;
    
    protected bool canAttack = true;
    protected bool dead = false;
    protected HealthBar healthbar;
    

    public void Setup(Team team, Node spawnNode)
    {

        myTeam = team;
        if(myTeam == Team.Team2)
        {
            spriteRenderer.flipX = true;
        }

        this.currentNode = spawnNode;
        transform.position = currentNode.worldPosition;
        currentNode.SetOccupied(true);
        healthbar = Instantiate(barPrefab, this.transform);
        baseHealth = 3;
        healthbar.Setup(this.transform, baseHealth);
    }
    
    public void TakeDamage(int amount){

        baseHealth -= amount;
        healthbar.UpdateBar(baseHealth);

        if(baseHealth <= 0){

            dead = true;
            CurrentNode.SetOccupied(false);
            GameManager.Instance.UnitDead(this); 

        }

    }
    
    protected void FindTarget(){
        var allEnemies = GameManager.Instance.GetEntitiesAgainst(myTeam);
        float minDistance = Mathf.Infinity;
        BaseEntity candidateTarget = null;

        foreach(BaseEntity e in allEnemies){
            if(Vector3.Distance(e.transform.position, this.transform.position) <= minDistance){
                minDistance = Vector3.Distance(e.transform.position, this.transform.position);
                candidateTarget = e;
             }
        }

        currentTarget = candidateTarget;
    }
    
    protected void GetInRange(){

        if(currentTarget == null)
            return;

        if(!moving){
            Node candidateDestination = null;
            List<Node> candidates = GridManager.Instance.GetNodesCloseTo(currentTarget.currentNode);
            candidates = candidates.OrderBy(x => Vector3.Distance(x.worldPosition, this.transform.position)).ToList();

            for(int i = 0; i < candidates.Count; i++){
                if(!candidates[i].isOccupied){
                    candidateDestination = candidates[i];
                    break;
                }
            }

            if(candidateDestination == null)
                return;

            var path = GridManager.Instance.GetPath(currentNode, candidateDestination);
            if(path == null || path.Count <=1)
                return;

            if(path[1].isOccupied)
                return;

            path[1].SetOccupied(true);
            destination = path[1];


        }

        moving = !MoveTowards();
        if(!moving){
            currentNode.SetOccupied(false);
            currentNode = destination;
        }
    }
    
    protected bool MoveTowards(){
        Vector3 direction = destination.worldPosition - this.transform.position;

        if(direction.sqrMagnitude <= 0.002f){
            transform.position = destination.worldPosition;
            return true;
        }

        this.transform.position += direction.normalized * movementSpeed * Time.deltaTime;
        return false;
    }

    protected virtual void Attack()
    {

        if (!canAttack)
            return;

        float waitBetweenAttack = 1 / attackSpeed;

        StartCoroutine(WaitAttackCoroutine(waitBetweenAttack));

    }

    IEnumerator WaitAttackCoroutine(float waitTime)
    {

        canAttack = false;
        yield return new WaitForSeconds(waitTime);
        canAttack = true;


    }
    
}