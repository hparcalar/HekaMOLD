app.controller('virtualPlantCtrl', ['$scope', '$http', 'Upload',
    function ($scope, $http, Upload) {
        $scope.modelObject = {};

        $scope.loadedObjectData = [];
        $scope.layoutObj = { layoutComponents: [] };
        $scope.availableMachines = [];
        $scope.editModeEnabled = true;
        $scope.groundSizeUnits = 150;
        $scope.selectedMachine = { Id: 0, MachineName: '' };
        $scope.canvas = document.createElement('canvas');
        $scope.canvas.id = 'renderCanvas';
        $scope.canvas.className = 'flex-fill';

        $scope.engine = new BABYLON.Engine($scope.canvas, true, { doNotHandleContextLost: true });
        $scope.engine.enableOfflineSupport = false;
        $scope.manager3d = null;
        $scope.scene = null;

        // #region DRAW FUNCTIONS
        $scope.createMachine = async (machineId) => {
            const machineObject = $scope.availableMachines.find(d => d.Id == machineId);
            if (machineObject.DesignPath == null) {
                toastr.error('Bu makine için görsel tasarım nesnesi yüklenmemiş.');
                return;
            }

            let mergedMesh = null;

            if ($scope.loadedObjectData.some(d => d.DesignPath == HOST_URL + 'Outputs/' + machineObject.DesignPath)) {
                mergedMesh = $scope.loadedObjectData.find(d => d.DesignPath == HOST_URL + 'Outputs/' + machineObject.DesignPath).mesh.clone();
            }
            else {
                const meshes = await BABYLON.SceneLoader.ImportMeshAsync(null, HOST_URL + 'Outputs/', machineObject.DesignPath, $scope.scene);
                mergedMesh = BABYLON.Mesh.MergeMeshes(meshes.meshes.filter(m => m.getClassName() == 'Mesh' && m.getTotalVertices() > 0),
                    true, true, null, true, true);
                $scope.scene.createDefaultEnvironment({
                    createSkybox: false,
                    enableGroundMirror: false,
                    createGround: false,
                });
                $scope.loadedObjectData.push({ DesignPath: HOST_URL + 'Outputs/' + machineObject.DesignPath, mesh: mergedMesh });
            }

            const machineSizeUnits = 150 * (0.3);

            const antiScalingSize = mergedMesh.getBoundingInfo().boundingBox.extendSize;
            const meshCenterWorld_Y = mergedMesh.getBoundingInfo().boundingBox.centerWorld.y;

            const scalingRateByX = machineSizeUnits / (antiScalingSize.x * 2);

            mergedMesh.scaling.scaleInPlace(scalingRateByX);

            mergedMesh.position = new BABYLON.Vector3(0, (meshCenterWorld_Y * scalingRateByX * -1)
                + (antiScalingSize.y * scalingRateByX), 0);
            // mergedMesh.computeWorldMatrix(true);
            // mergedMesh.bakeCurrentTransformIntoVertices();

            const pointerDragBehavior = new BABYLON.PointerDragBehavior({ dragPlaneNormal: new BABYLON.Vector3(0, 1, 0) });
            pointerDragBehavior.useObjectOrientationForDragging = false;
            pointerDragBehavior.enabled = $scope.editModeEnabled;
            mergedMesh.addBehavior(pointerDragBehavior);

            // const objLabel = createLabelForWorldObject(machineObject.name, 'Red', { x: 0, y: 10, z: -1.5 });

            // mergedMesh?.addChild(objLabel);

            const newComponent = {
                code: machineObject.MachineCode,
                name: machineObject.MachineName,
                scalingData: { x: 1, y: 1, z: 1 },
                positionData: { x: 1, y: 1, z: 1 },
                rotationData: { x: 0, y: 0, z: 0 },
                machineId: machineObject.Id,
            };

            mergedMesh.metadata = newComponent;

            $scope.layoutObj.layoutComponents.push(newComponent);
            $scope.availableMachines.splice($scope.availableMachines.indexOf(machineObject), 1);
        };

        $scope.drawMachine = async (machineObject) => {
            
            machineObject.scalingData = JSON.parse(machineObject.ScalingData);
            machineObject.machineId = machineObject.MachineId;
            machineObject.positionData = JSON.parse(machineObject.PositionData);
            if (machineObject.RotationData)
                machineObject.rotationData = JSON.parse(machineObject.RotationData);
            else
                machineObject.rotationData = { x: 0, y: 0, z: 0 };

            let mergedMesh = null;

            if ($scope.loadedObjectData.some(d => d.DesignPath == machineObject.DesignPath)) {
                mergedMesh = $scope.loadedObjectData.find(d => d.DesignPath == machineObject.DesignPath).mesh.clone();
            }
            else {
                const meshes = await BABYLON.SceneLoader.ImportMeshAsync(null, HOST_URL + 'Outputs/', machineObject.DesignPath, $scope.scene);
                mergedMesh = BABYLON.Mesh.MergeMeshes(meshes.meshes.filter(m => m.getClassName() == 'Mesh' && m.getTotalVertices() > 0),
                    true, true, null, true, true);
                $scope.scene.createDefaultEnvironment({
                    createSkybox: false,
                    enableGroundMirror: false,
                    createGround: false,
                });
                $scope.loadedObjectData.push({ DesignPath: HOST_URL + 'Outputs/' + machineObject.DesignPath, mesh: mergedMesh });
            }

            const machineSizeUnits = 150 * (0.3);

            const antiScalingSize = mergedMesh.getBoundingInfo().boundingBox.extendSize;
            const meshCenterWorld_Y = mergedMesh.getBoundingInfo().boundingBox.centerWorld.y;

            const scalingRateByX = machineSizeUnits / (antiScalingSize.x * 2);

            mergedMesh.scaling.scaleInPlace(machineObject.scalingData.x);

            if (machineObject.rotationData != null)
                mergedMesh.rotation = new BABYLON.Vector3(machineObject.rotationData.x, machineObject.rotationData.y, machineObject.rotationData.z);

            mergedMesh.position = new BABYLON.Vector3(machineObject.positionData.x, (meshCenterWorld_Y *
                (scalingRateByX * machineObject.scalingData.x) * -1)
                + (antiScalingSize.y * (scalingRateByX * machineObject.scalingData.x)), machineObject.positionData.z);
            // mergedMesh.computeWorldMatrix(true);
            // mergedMesh.bakeCurrentTransformIntoVertices();

            const pointerDragBehavior = new BABYLON.PointerDragBehavior({ dragPlaneNormal: new BABYLON.Vector3(0, 1, 0) });
            pointerDragBehavior.useObjectOrientationForDragging = false;
            pointerDragBehavior.enabled = $scope.editModeEnabled;
            mergedMesh.addBehavior(pointerDragBehavior);

            // const objLabel = createLabelForWorldObject(machineObject.name, 'Red', {
            //   y: machineObject.positionData.y + 5,
            //   z: machineObject.positionData.z + 5,
            //   x: machineObject.positionData.x + 5,
            // });

            // mergedMesh?.addChild(objLabel);

            mergedMesh.metadata = machineObject;

            // mergedMesh.optimizeIndices(() => {
            //   mergedMesh.simplify([
            //     { distance: 250, quality: 0.8 }, { distance: 300, quality: 0.5 },
            //     { distance: 400, quality: 0.3 }, { distance: 500, quality: 0.1 }], false, SimplificationType.QUADRATIC, () => {

            // 	  });
            // });
        };
        // #endregion

        $scope.createScene = function () {
            $scope.scene = new BABYLON.Scene($scope.engine);
            $scope.scene.clearCachedVertexData();
            $scope.scene.cleanCachedTextureBuffer();

            // Create the 3D UI manager
            $scope.manager3d = new BABYLON.GUI.GUI3DManager($scope.scene);

            // Initialize GizmoManager
            const gizmoManager = new BABYLON.GizmoManager($scope.scene);

            const camera = new BABYLON.ArcRotateCamera('camera', -Math.PI / 2, Math.PI / 2.5, 75, new BABYLON.Vector3(0, 20, 0), $scope.scene);
            camera.attachControl($scope.canvas, true, true);
            camera.panningSensibility = 50;
            camera.wheelPrecision = 1;
            camera.setTarget(BABYLON.Vector3.Zero());
            const light = new BABYLON.HemisphericLight('light', new BABYLON.Vector3(0, 1, 0), $scope.scene);

            const highlightLayer = new BABYLON.HighlightLayer('matH_Layer', $scope.scene);
            $scope.scene.clearColor = BABYLON.Color3.FromHexString('#ffffff');

            let currentMesh, gizmo;
            const utilLayer = new BABYLON.UtilityLayerRenderer($scope.scene);
            utilLayer.utilityLayerScene.autoClearDepthAndStencil = false;

            const ground = BABYLON.MeshBuilder.CreateGround('ground', { width: 1000, height: 1000 }, $scope.scene);

            // const groundMat = new StandardMaterial('groundMat', scene);
            // groundMat.diffuseTexture = new Texture('/arroway-tiles.jpg', scene);
            // ground.material = groundMat;
            const litMaterial = new BABYLON.StandardMaterial('litMat', $scope.scene);
            litMaterial.diffuseColor = BABYLON.Color3.FromHexString('#4b4b4b');
            ground.material = litMaterial;
            ground.position = new BABYLON.Vector3(0, 0, 0);

            const clearSelectedObjectEffects = () => {
                if (currentMesh != null) {
                    if (currentMesh.parent)
                        currentMesh = currentMesh.parent;

                    if (gizmo)
                        gizmo.dispose();

                    currentMesh = null;
                    highlightLayer.removeAllMeshes();
                }
            };

            // DRAW LOADED OBJECTS
            $scope.layoutObj.layoutComponents.forEach((comp) => {
                $scope.drawMachine(comp);
            });

            // #region SCENE INTERACTION MANAGER
            $scope.scene.onPointerObservable.add((pointerInfo) => {
                switch (pointerInfo.type) {
                    case BABYLON.PointerEventTypes.POINTERUP:
                        if (pointerInfo.event.which == 1) {
                            if (pointerInfo.pickInfo.hit && pointerInfo.pickInfo.pickedMesh != ground) {
                                if (currentMesh != pointerInfo.pickInfo.pickedMesh) {
                                    clearSelectedObjectEffects();

                                    currentMesh = pointerInfo.pickInfo.pickedMesh;
                                    const meshSize = currentMesh.getBoundingInfo().boundingBox.extendSize;

                                    if ($scope.editModeEnabled == true) {
                                        gizmo = new BABYLON.BoundingBoxGizmo(BABYLON.Color3.FromHexString('#0984e3'), utilLayer);
                                        gizmo.setEnabledRotationAxis('y');
                                        gizmo.scaleBoxSize = 1;
                                        gizmo.rotationSphereSize = 1;
                                        gizmo.attachedMesh = currentMesh;
                                        gizmo.updateGizmoRotationToMatchAttachedMesh = true;
                                        gizmo.onRotationSphereDragEndObservable.add((event) => {
                                            const rotationData = currentMesh.rotationQuaternion.toEulerAngles();
                                            currentMesh.metadata.rotationData.x = rotationData.x;
                                            currentMesh.metadata.rotationData.y = rotationData.y;
                                            currentMesh.metadata.rotationData.z = rotationData.z;
                                        });
                                    }

                                    highlightLayer.addMesh(currentMesh, BABYLON.Color3.Green());

                                    if (currentMesh.metadata.componentTypeId == 2) {
                                        //emit('click-machine', currentMesh.metadata.id, currentMesh?.metadata.name);
                                        $scope.selectedMachine = currentMesh?.metadata;
                                    }
                                    else if (currentMesh.metadata.componentTypeId == 1) {
                                        //emit('click-department', currentMesh.metadata.id, currentMesh?.metadata.name);
                                        $scope.selectedMachine = { Id: 0, MachineName: '' };
                                    }
                                    else {
                                        //emit('click-outside');
                                        $scope.selectedMachine = { Id: 0, MachineName: '' };
                                    }
                                }
                            }
                            else {
                                $scope.selectedMachine = { Id: 0, MachineName: '' };
                                clearSelectedObjectEffects();
                                //emit('click-outside');
                            }
                        }
                        break;
                }
            });
            // #endregion

            // #region SCENE KEY OBSERVER
            $scope.scene.onKeyboardObservable.add((kbInfo) => {
                switch (kbInfo.type) {
                    case BABYLON.KeyboardEventTypes.KEYUP:
                        if (kbInfo.event.keyCode == 46) {
                            if ($scope.editModeEnabled == true) {
                                if (currentMesh.metadata.componentTypeId == 2
                                    && confirm('Bu nesneyi kaldırmak istediğinizden emin misiniz?') == true) {
                                    const delObj = currentMesh.metadata;

                                    //const delObjIndex = $scope.layoutObj.layoutComponents.indexOf(delObj);
                                    //$scope.layoutObj.layoutComponents.splice(delObjIndex, 1);

                                    if (gizmo)
                                        gizmo.dispose();

                                    currentMesh.dispose();
                                    currentMesh = null;
                                }
                            }
                        }
                        break;
                }
            });
            // #endregion

            return $scope.scene;
        };

        // #region CRUD
        $scope.saveStatus = 0;
        $scope.saveModel = async () => {
            /*editModeEnabled.value = !editModeEnabled.value;*/
            var componentList = [];

            for (var i = 0; i < $scope.scene.meshes.length; i++) {
                const mesh = $scope.scene.meshes[i];
                if (mesh.behaviors.length > 0) {
                    const ddBhv = mesh.behaviors[0];
                    if (ddBhv)
                        ddBhv.enabled = $scope.editModeEnabled;
                }

                // save layout changes to db
                const componentObj = mesh.metadata;
                if (componentObj) {
                    console.log(componentObj);
                    componentObj.scalingData = { x: mesh.scaling.x, y: mesh.scaling.y, z: mesh.scaling.z };
                    componentObj.positionData = { x: mesh.position.x, y: mesh.position.y, z: mesh.position.z };

                    componentList.push({
                        MachineId: componentObj.machineId,
                        RotationData: JSON.stringify(componentObj.rotationData),
                        ScalingData: JSON.stringify(componentObj.scalingData),
                        PositionData: JSON.stringify(componentObj.positionData),
                    });
                }
            }

            $scope.saveStatus = 1;

            $http.post(HOST_URL + 'Layout/SaveModel', componentList, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.saveStatus = 0;

                        if (resp.data.Result == true) {
                            toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                            //$scope.bindModel(resp.data.RecordId);
                        }
                        else
                            toastr.error(resp.data.ErrorMessage, 'Hata');
                    }
                }).catch(function (err) { });
        };

        $scope.bindModel = function () {
            var prms = new Promise(function (resolve, reject) {
                $http.get(HOST_URL + 'Layout/BindModel', {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.layoutObj.layoutComponents = resp.data.Items;
                            $scope.availableMachines = resp.data.Machines;

                            // START SCENE
                            $scope.scene = $scope.createScene();
                            resolve();
                        }
                    }).catch(function (err) { console.log(err); });
            });

            return prms;
        }
        // #endregion

        // #region DRAG & DROP EVENTS
        $scope.dragOverOfCanvas = (event) => {
            event.preventDefault();
        };

        $scope.dropOverCanvas = (event) => {
            if ($scope.editModeEnabled == true) {
                const macId = event.dataTransfer?.getData('machineObject');
                if (macId != null)
                    $scope.createMachine(macId);
            }
            else {
                toastr.error('Düzenleme modu aktif değil.');
            }
        };
        // #endregion

        // ON LOAD EVENTS
        $scope.bindModel().then(() => {
            if (document.getElementById('renderCanvas') == null) {
                document.getElementById('layoutContainer').appendChild($scope.canvas);
                $scope.canvas.ondragover = $scope.dragOverOfCanvas;
                $scope.canvas.ondrop = $scope.dropOverCanvas;
                $scope.engine.resize();
            }

            // Register a render loop to repeatedly render the scene
            $scope.engine.runRenderLoop(() => {
                $scope.scene.render();
            },
            );

            // Watch for browser/canvas resize events
            window.addEventListener('resize', () =>
                $scope.engine.resize(),
            );
        });
    }]);

const dragStartOfMachine = (event) => {
    var machineId = parseInt($(event.srcElement).attr('data-id'));
    event.dataTransfer.setData('machineObject', machineId);
};