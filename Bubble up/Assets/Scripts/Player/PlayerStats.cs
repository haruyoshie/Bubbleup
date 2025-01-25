using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public Transform Sprite;

    public CircleCollider2D Collider;

    public float MaxLife = 10;

    public float JumpForce = 1;

    public float MaxFallSpeed;

    public float MaxHorizontalSpeed;

    public float LimitDistnaceToFail = 20f;

    private float _jumpTimeEnergy;
    private float _currentLife;
    private float _currentFallSpeed;
    private float _currentHorizontalSpeed;

    public float JumpTimeEnergy { get => _jumpTimeEnergy; set => _jumpTimeEnergy = value; }
    public float CurrentLife { get => _currentLife; set => _currentLife = value; }
    public float CurrentFallSpeed { get => _currentFallSpeed; set => _currentFallSpeed = value; }
    public float CurrentHorizontalSpeed { get => _currentHorizontalSpeed; set => _currentHorizontalSpeed = value; }
}
