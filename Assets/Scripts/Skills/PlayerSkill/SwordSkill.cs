using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class SwordSkill : Skills
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce Info")]
    [SerializeField] private int bounceAmmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed = 10;


    [Header("Pierce Info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin Info")]
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 2;
    [SerializeField] private float hitCooldown = 0.35f;

    [Header("Skill Info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float returnSpeed = 7;
    [SerializeField] private float freezeTimeDuration = 0.7f;
    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();
        GenerateDots();
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            Vector2 aim = AimDirection();
            finalDir = new Vector2(aim.normalized.x * launchForce.x, aim.normalized.y * launchForce.y);
        }

        if (Input.GetKey(KeyCode.R))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }
    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        SwordSkillController newSwordScript = newSword.GetComponent<SwordSkillController>();

        if (swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
            newSwordScript.SetupBounce(true, bounceAmmount, bounceSpeed);
        }
        else if (swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
            newSwordScript.SetupPierce(pierceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
        }

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);
        player.AssignNewSword(newSword);
        DotsActive(false);
    }


    #region Aim Sword
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;
        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 aim = AimDirection();
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            aim.normalized.x * launchForce.x,
            aim.normalized.y * launchForce.y) * t + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);
        return position;
    }
    #endregion
}
