app.controller('virtualPlantCtrl', ['$scope', '$http', 'Upload',
    function ($scope, $http, Upload) {
        $scope.modelObject = {};

        $scope.loadedObjectData = [];
        $scope.layoutObj = { layoutComponents: [] };
        $scope.availableMachines = [];
        $scope.editModeEnabled = false;
        $scope.groundSizeUnits = 150;
        $scope.selectedMachine = { Id: 0, MachineName: '' };
        $scope.canvas = document.createElement('canvas');
        $scope.canvas.id = 'renderCanvas';
        $scope.canvas.className = 'h-75';

        $scope.engine = new BABYLON.Engine($scope.canvas, true, { doNotHandleContextLost: true });
        $scope.engine.enableOfflineSupport = false;
        $scope.manager3d = null;
        $scope.scene = null;

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
            camera.wheelPrecision = 5;
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
            //$scope.layoutObj.value.layoutComponents.forEach((comp) => {
            //    if (comp.componentTypeId === 1)
            //        drawDepartment(comp);

            //    else if (comp.componentTypeId === 2)
            //        drawMachine(comp);
            //});

            // #region SCENE INTERACTION MANAGER
            $scope.scene.onPointerObservable.add((pointerInfo) => {
                switch (pointerInfo.type) {
                    case BABYLON.PointerEventTypes.POINTERUP:
                        if (pointerInfo.event.which === 1) {
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
                        if (kbInfo.event.keyCode === 46) {
                            if ($scope.editModeEnabled === true) {
                                if (currentMesh.metadata.componentTypeId === 2
                                    && confirm('Bu nesneyi kaldırmak istediğinizden emin misiniz?') === true) {
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

        $scope.saveStatus = 0;
        $scope.saveModel = function () {
            $scope.saveStatus = 1;

            $http.post(HOST_URL + 'Layout/SaveModel', $scope.modelObject, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.saveStatus = 0;

                        if (resp.data.Status == 1) {
                            toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                            $scope.bindModel(resp.data.RecordId);
                        }
                        else
                            toastr.error(resp.data.ErrorMessage, 'Hata');
                    }
                }).catch(function (err) { });
        }
        $scope.bindModel = function () {
            var prms = new Promise(function (resolve, reject) {
                $http.get(HOST_URL + 'Layout/BindModel', {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.layoutObj.layoutComponents = resp.data;

                            // START SCENE
                            $scope.scene = $scope.createScene();
                            resolve();
                        }
                    }).catch(function (err) { console.log(err); });
            });

            return prms;
        }

        // ON LOAD EVENTS
        $scope.bindModel().then(() => {
            if (document.getElementById('renderCanvas') == null) {
                document.getElementById('layoutContainer').appendChild($scope.canvas);
                //$scope.canvas.ondragover = dragOverOfCanvas;
                //$scope.canvas.ondrop = dropOverCanvas;
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