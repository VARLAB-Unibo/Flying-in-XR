#  [**Flying in XR: Bridging Desktop Applications in eXtended Reality through Deep Learning**]()

[Lorenzo Stacchio](https://www.unibo.it/sitoweb/lorenzo.stacchio2),
[Giuseppe Di Maria](https://www.linkedin.com/in/giuseppe-di-maria-6bb5a4170/?originalSubdomain=it),
[Gustavo Marfia](https://github.com/qp-qp)<br/>

| [IEEE VR '24 Oral Presentation at OAT Workshop](https://openvrlab.github.io/) | [paper](https://www.researchgate.net/publication/377635198_Flying_in_XR_Bridging_Desktop_Applications_in_eXtended_Reality_through_Deep_Learning) | [project page]() | [video](https://drive.google.com/file/d/1vtoUi0iwBAnRYz_IbpJH_X-MT0ZEcS1Q/view?usp=sharing)

![framework_di_maria (1)](https://github.com/VARLAB-Unibo/Flying-from-2D-to-3D/assets/142809173/0703913c-44a2-48f2-852f-a96e42281a23)



## Abstact
The expanding realm of eXtended Reality (XR) has witnessed a surge in 3D experiences across diverse domains, undergoing significant transformations to define novel experiences. However, such experiences are often built from scratch, as there is a lack of tools that support augmenting existing non-immersive interfaces, like Desktop games and simulators, directly into XR. Such shortage is particularly exacerbated in the case of  Mixed Reality (MR). Motivated by this, we present a novel middleware, Flying In XR (FIXR), leveraging Deep Learning to visualize and interact with desktop application views into XR. To demonstrate the flexibility of such an approach, we applied FIXR to a commercial Desktop flight simulator, supporting an MR experience. It is worth noticing that FIXR could be adapted to communicate with any desktop software with a camera that moves along the depth axis, opening new paths to enable user experiences in XR for a wide spectrum of applications.


## Requirements
* [Unity 2021.3.21f1](https://unity.com/releases/editor/archive#download-archive-2021)
* [SteamVR v2.7.3](https://github.com/ValveSoftware/steamvr_unity_plugin/releases/tag/2.7.3) 


## Comparison with Related works
| Related Work                                     | MXRP       | GLI        | NI         | LL         | MRS        | OS         |
| ------------------------------------------------- | ---------- | ---------- | ---------- | ---------- | ---------- | ---------- |
| [7](https://ieeexplore.ieee.org/document/9213691/), [8](https://ieeexplore.ieee.org/document/9576155/)                        | :x:        | :x:        | :x:        | :heavy_check_mark: | :heavy_check_mark: | :x:        |
| [22](https://www.techviz.net), [41](https://www.more3d.com/moreviz-vr-bridge), [47](https://www.vorpx.com/features/)    | :x:        | :x:        | ≈          | :heavy_check_mark: | :x:        | :x:        |
| [48](https://ieeexplore.ieee.org/document/9288815)                        | :heavy_check_mark: | :x:        | :x:        | :heavy_check_mark: | :heavy_check_mark: | :x:        |
| [46](https://varjo.com/solutions/training-and-simulation/)                             | :x:        | :x:        | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :x:        |
| **FIXR**                                         | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | ≈          | :heavy_check_mark: | :heavy_check_mark: |

***MXRP**: Modular eXtended Reality Projection*
***GLI**: Graphic Library Independence*
***NI**: Natural Interactions*
***LL**: Low Latency*
***MRS**: Mixed Reality Support*
***OS**: Open-Source*


## Main Features


## Demo

The demo is in the ``````.


## Cite the work 
```
@inproceedings{stacchio2024flying,
  title={Flying in XR: Bridging Desktop applications in eXtended Reality through Deep Learning},
  author={Stacchio, Lorenzo and Di Maria, Giuseppe and Marfia, Gustavo},
  booktitle={2024 IEEE Conference on Virtual Reality and 3D User Interfaces Abstracts and Workshops (VRW)},
  pages={to appear},
  year={2024},
  organization={IEEE}
}
```
