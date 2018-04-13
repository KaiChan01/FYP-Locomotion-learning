# FYP-Locomotion-learning

## Introduction
This repo holds the scource code for my DIT Computer Science Final Year Project

In my project, I explore AI Algorithms that can help evolve interesting locomotion for virtual creatures in Unity. 

To achieve my goal, I've utilized the Genetic Algorithm and Neural Networks. The Genetic Algorithm is responsible for evolving the population of neural networks. Evolving neural networks are tested by assigned them to a creature object in Unity. The neural network will instruct the joints of the creature in an attempt to locomote. A fitness function is implemented to measure the creature's locomotion performance.

Evolved networks  are persisted into a JSON file, these files will hold infomation on what type of creature was train, and the information of the neural net itself. 

The framework can also deserialize the persisted files to recreate the creature and the locomotion that was evolved.

## Demonstrations

**Training demonstration**
[![Video](http://i3.ytimg.com/vi/o0f3W0l6RBQ/maxresdefault.jpg)](https://www.youtube.com/watch?v=o0f3W0l6RBQ)

**Demonstration of loading various trained creatures and letting them locomote together**
[![Video](http://i3.ytimg.com/vi/W6KsiACIOUA/maxresdefault.jpg)](https://www.youtube.com/watch?v=W6KsiACIOUA)

**Demonstration of how the trained locomotion can be used with player controls**
[![Video](http://i3.ytimg.com/vi/W6KsiACIOUA/maxresdefault.jpg)](https://www.youtube.com/watch?v=W6KsiACIOUA)
